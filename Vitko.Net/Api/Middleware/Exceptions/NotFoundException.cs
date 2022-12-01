using System.Net;

namespace Vitko.Net.Api.Middleware.Exceptions;

public class NotFoundException : AbstractApiException
{
    public NotFoundException(string message) : base(message, HttpStatusCode.NotFound)
    {
    }
    
    public NotFoundException(string message, Exception innerException) : base(message, HttpStatusCode.NotFound, innerException)
    {
    }
}