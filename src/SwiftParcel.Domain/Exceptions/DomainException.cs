using System.Net;

namespace SwiftParcel.Domain.Exceptions;

/// <summary>
/// Represents a domain-specific exception that can be thrown
/// when business rule is violated or a domain-related error occurs.
/// </summary>
public abstract class DomainException : Exception
{
    public string Code { get; }
    public HttpStatusCode StatusCode { get; }
    
    protected DomainException(string code, string message, 
        HttpStatusCode statusCode = HttpStatusCode.BadRequest) 
        : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }
}