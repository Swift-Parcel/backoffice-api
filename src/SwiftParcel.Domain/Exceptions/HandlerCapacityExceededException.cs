using System.Net;

namespace SwiftParcel.Domain.Exceptions;

public class HandlerCapacityExceededException : DomainException
{
    public HandlerCapacityExceededException() 
        : base("Handler capacity exceeded.", HttpStatusCode.Conflict)
    {
    }
}