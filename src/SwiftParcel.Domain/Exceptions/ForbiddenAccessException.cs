using System.Net;

namespace SwiftParcel.Domain.Exceptions;

public class ForbiddenAccessException : DomainException
{
    public ForbiddenAccessException(string message) 
        : base("forbidden_access", message, HttpStatusCode.Forbidden)
    {
    }
}