using System.Net;

namespace Vitko.Net.Api.Middleware.Exceptions;

/// <summary>
/// Exception that will result in the middleware returning 500 to the client.
/// For example, can be thrown in catch blocks to return a custom error message.
/// </summary>
public class InternalServerException : AbstractApiException
{
    public InternalServerException(string message) : base(message, HttpStatusCode.InternalServerError)
    {
    }
    
    public InternalServerException(string message, Exception innerException) : base(message, HttpStatusCode.InternalServerError, innerException)
    {
    }
}