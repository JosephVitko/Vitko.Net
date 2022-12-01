using System.Net;

namespace Vitko
    .Net.Api.Middleware.Exceptions;

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