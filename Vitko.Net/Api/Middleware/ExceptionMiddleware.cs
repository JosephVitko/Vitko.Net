using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Vitko.Net.Api.Middleware.Exceptions;

namespace Vitko.Net.Api.Middleware;

/// <summary>
/// Middleware used for handling exceptions in the application.
/// If the exception extends AbstractApiException, the appropriate status code is returned.
/// Otherwise, the status code 500 is returned.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError; // 500 if unexpected

        if (exception is AbstractApiException apiException)
        {
            code = apiException.StatusCode;
        }

        var result = JsonConvert.SerializeObject(new { error = exception.Message });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        await context.Response.WriteAsync(result);
    }
}