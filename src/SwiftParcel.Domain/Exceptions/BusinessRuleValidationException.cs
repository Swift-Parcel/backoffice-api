using System.Net;

namespace SwiftParcel.Domain.Exceptions;

public class BusinessRuleValidationException : DomainException
{
    public BusinessRuleValidationException(string code, string message) 
        : base(code, message, HttpStatusCode.BadRequest)
    {
    }
}