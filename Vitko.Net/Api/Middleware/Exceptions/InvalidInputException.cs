using System.Net;

namespace Vitko.Net.Api.Middleware.Exceptions;

/// <summary>
/// Exception that will result in the middleware returning 400 to the client.
/// For example, can be thrown when validation of user input fails.
/// </summary>
public class InvalidInputException : AbstractApiException
{
    public InvalidInputException(string message) : base(message, HttpStatusCode.BadRequest)
    {
    }
    
    public InvalidInputException(string message, Exception innerException) : base(message, HttpStatusCode.BadRequest, innerException)
    {
    }
}