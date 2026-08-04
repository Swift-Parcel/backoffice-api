using System.Net;
using SwiftParcel.Domain.Exceptions;

namespace SwiftParcel.Domain.Exceptions;

public class CapacityExceededException : DomainException
{
    public CapacityExceededException(int handlerId, int maxCases) 
        : base(
            code: "handler_capacity_exceeded", 
            message: $"Handler {handlerId} has reached their maximum capacity of {maxCases} active cases.",
            statusCode: HttpStatusCode.Conflict)
    {
    }
}