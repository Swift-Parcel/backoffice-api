using System.Net;

namespace SwiftParcel.Domain.Exceptions;

public class ForbiddenAccessException : DomainException
{
    public ForbiddenAccessException(string message) 
        : base(message, HttpStatusCode.Forbidden)
    {
    }
}