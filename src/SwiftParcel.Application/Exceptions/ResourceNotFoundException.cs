using System.Net;

namespace SwiftParcel.Domain.Exceptions;

public class ResourceNotFoundException : DomainException
{
    public ResourceNotFoundException(string resourceName, string identifier) 
        : base(
            code: $"{resourceName.ToLower()}_not_found", 
            message: $"{resourceName} with identifier '{identifier}' was not found.",
            statusCode: HttpStatusCode.NotFound) 
    {
    }
}