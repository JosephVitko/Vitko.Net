using System.Net;

namespace Vitko.Net.Api.Middleware.Exceptions;

/// <summary>
/// Exception that will result in the middleware returning 404 to the client.
/// For example, can be thrown when a resource is not found.
/// </summary>
public class NotFoundException : AbstractApiException
{
    public NotFoundException(string message) : base(message, HttpStatusCode.NotFound)
    {
    }
    
    public NotFoundException(string message, Exception innerException) : base(message, HttpStatusCode.NotFound, innerException)
    {
    }
}