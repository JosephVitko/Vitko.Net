# Vitko.Net
This is my personal library for making ASP.NET Core web apps with Azure integration.

The library abstracts CRUD operations with Azure Blob Storage, CosmosDB, and Active Directory. It also contains some middleware and startup helpers to expedite development.

The current version supports ASP.NET Core 6.40

## How to use

### Configuration
The included startup helper automatically initializes the required services by looking in appsettings.json.

Three main properties can be configured:
- `ServiceConfig`: Specifies which Azure services to use. This is done by specifying `EnableBlobService`, `EnableCosmosService`, and `EnableUserService` with a boolean value. 
- `AuthScheme`: Within the service config, the scheme for authentication can be specified. This should have the same name as a configuration section, such as `AzureAd` or `AzureAdB2C`. If enabled, the user service will also use the specified directory for user management. However, currently only Azure AD B2C is supported for user management.
- `AllowedOrigins`: This should be a list of urls that are allowed to access the API. This is used for configuring CORS.

Here is an example appsettings.json file:
```json
{
  "ServiceConfig": {
    "AuthScheme": "AzureAdB2C",
    "EnableBlobService": true,
    "EnableCosmosService": true,
    "EnableUserService": true
  },
  "AllowedOrigins": [
    "https://localhost:3000"
  ],
  "AzureAdB2C": {
    "Instance": "https://login.microsoftonline.com/tfp/",
    "Domain": "mydomain.onmicrosoft.com",
    "ClientId": "myclientid"
  }
}
```

### Startup
To use the startup helper, import the library and run your app in the following way:
```c#
public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseStartup<Startup>()
                    .UseSetting(WebHostDefaults.ApplicationKey, typeof(Program).GetTypeInfo().Assembly.FullName);
            });
```

### Middleware
The startup helper automatically adds middleware to the pipeline for exception handling.

Three exceptions, each extending `AbstractApiException`, are included:
- `InternalServerException`: Will return 500 to the client when thrown
- `BadRequestException`: Will return 400 to the client when thrown
- `NotFoundException`: Will return 404 to the client when thrown

These exceptions also allow you to specify custom error messages that will be returned to the client. They can be used to more elegantly handle errors in the API.

### Services
In your models, you can add code like this to enable CRUD operations with Azure:
```c#
private const string ContainerName = "devices";
    private static ICrudService<Device>? _service;
    public static ICrudService<Device> Service
    {
        get
        {
            _service ??= CosmosDbServiceFactory.Instance.CreateService<Device>(ContainerName);
            return _service;
        }

        set => _service = value;
    }
```

This will create a service that can be used to perform CRUD operations on the model. The service will automatically be initialized with the configuration specified in appsettings.json.

```c#
await Device.Service.GetItemAsync("mydeviceid");
```

For users, you must also add methods for conversion between the user model and the Microsoft Graph user class.
```c#
 _service ??= UserServiceFactory.B2CInstance.CreateService<B2CUser>(null, ConvertUserToType, ConvertTypeToUser);
 
 public static explicit operator B2CUser(Microsoft.Graph.User user) {
    return new B2CUser(user.Id, user.DisplayName, user.UserPrincipalName, user.Mail);
 }
 
public static explicit operator Microsoft.Graph.User(B2CUser user) {
    return new Microsoft.Graph.User {
        Id = user.Id,
        DisplayName = user.DisplayName,
        UserPrincipalName = user.Email,
        Mail = user.Email
    };
}

public static B2CUser ConvertUserToType(Microsoft.Graph.User user) {
    return (B2CUser) user;
}

public static Microsoft.Graph.User ConvertTypeToUser(B2CUser user) {
    return (Microsoft.Graph.User) user;
}
```
 