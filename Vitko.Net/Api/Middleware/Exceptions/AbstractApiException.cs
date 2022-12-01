using System.Net;

namespace Vitko.Net.Api.Middleware.Exceptions;

/// <summary>
/// Abstract exception that contains a status code and message so the middleware knows what to return to the client.
/// </summary>
public abstract class AbstractApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    
    public AbstractApiException(string message, HttpStatusCode statusCode, Exception innerException) : base(message, innerException)
    {
        StatusCode = statusCode;
    }
    
    public AbstractApiException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
    
}