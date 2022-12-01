using System.Net;

namespace Vitko.Net.Api.Middleware.Exceptions;

public class InvalidInputException : AbstractApiException
{
    public InvalidInputException(string message) : base(message, HttpStatusCode.BadRequest)
    {
    }
    
    public InvalidInputException(string message, Exception innerException) : base(message, HttpStatusCode.BadRequest, innerException)
    {
    }
}