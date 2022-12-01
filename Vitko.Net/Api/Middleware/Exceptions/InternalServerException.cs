using System.Net;

namespace Vitko.Net.Api.Middleware.Exceptions;

public class InternalServerException : AbstractApiException
{
    public InternalServerException(string message) : base(message, HttpStatusCode.InternalServerError)
    {
    }
    
    public InternalServerException(string message, Exception innerException) : base(message, HttpStatusCode.InternalServerError, innerException)
    {
    }
}