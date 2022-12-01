using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Vitko.Net.Api.Middleware;
using Vitko.Net.Azure.Blob;
using Vitko.Net.Azure.Cosmos;
using Vitko.Net.Azure.Graph;

namespace Vitko.Net.Api;

/// <summary>
/// Default startup class that automatically configures:
/// - Authentication if an "AuthScheme" is specified in the configuration
/// - Blob, CosmosDB, and Graph Services if enabled in the configuration
/// </summary>
public class Startup
{
    public IConfiguration Configuration { get; }
    
    public Startup(
        IConfiguration configuration
    )
    {
        Configuration = configuration;
    }

    /// <summary>
    /// Configures authentication if an "AuthScheme" is specified in the configuration
    /// as well as the Blob, CosmosDB, and Graph Services if enabled in the configuration
    /// </summary>
    /// <param name="services"></param>
    /// <exception cref="Exception"></exception>
    public void ConfigureServices(IServiceCollection services)
    {
        IConfigurationSection serviceConfigSection = Configuration.GetSection("ServiceConfig");

        // This is required to be instantiated before the OpenIdConnectOptions starts getting configured.
        // By default, the claims mapping will map claim names in the old format to accommodate older SAML applications.
        // 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' instead of 'roles'
        // This flag ensures that the ClaimsIdentity claims collection will be built from the claims in the token
        // JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        // Adds Microsoft Identity platform (AAD v2.0) support to protect this Api
        string? authScheme = serviceConfigSection["AuthScheme"];
        if (authScheme != null)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(options =>
                    {
                        Configuration.Bind(authScheme, options);

                        options.TokenValidationParameters.NameClaimType = "name";
                    },
                    options => { Configuration.Bind(authScheme, options); });
        }
        
        

        if (serviceConfigSection["EnableUserService"] == "True")
        {
            if (authScheme == null)
            {
                throw new Exception("UserServiceEnabled is true but AuthScheme is null");
            }
            if (authScheme.Equals("AzureAdB2C"))
            {
                var b2CConfiguration = Configuration.GetSection("AzureAdb2C");
                UserServiceFactory.InitializeB2C(
                    b2CConfiguration.GetSection("Scopes").Get<string[]>(),
                    b2CConfiguration["ClientId"],
                    b2CConfiguration["TenantId"],
                    b2CConfiguration["ClientSecret"],
                    b2CConfiguration.GetSection("UserFields").Get<string[]>()
                );
            }
            else
            {
                throw new Exception($"Unsupported AuthScheme: {authScheme}");
            }
        }
        
        if (serviceConfigSection["EnableCosmosService"] == "True")
        {
            string cosmosConnectionString = Configuration.GetSection("CosmosDb").GetValue<string>("ConnectionString");
            string cosmosDatabaseName = Configuration.GetSection("CosmosDb").GetValue<string>("DatabaseName");
            CosmosDbServiceFactory.Initialize(cosmosConnectionString, cosmosDatabaseName);
        }
        
        if (serviceConfigSection["EnableBlobService"] == "True")
        {
            BlobServiceFactory.Initialize(Configuration.GetSection("BlobStorage").GetValue<string>("ConnectionString"));
        }

        services.AddCors();
        
        services.AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            // Since IdentityModel version 5.2.1 (or since Microsoft.AspNetCore.Authentication.JwtBearer version 2.2.0),
            // PII hiding in log files is enabled by default for GDPR concerns.
            // For debugging/development purposes, one can enable additional detail in exceptions by setting IdentityModelEventSource.ShowPII to true.
            // Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();



        app.UseCors(x => x.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(Configuration.GetSection("AllowedOrigins").Get<string[]>()));

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ExceptionMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

}