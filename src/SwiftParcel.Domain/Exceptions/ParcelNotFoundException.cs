using System.Net;

namespace SwiftParcel.Domain.Exceptions;

public class ParcelNotFoundException : DomainException
{
    public ParcelNotFoundException(string trackingNumber)
        : base($"Parcel with tracking number '{trackingNumber}' was not found.", 
            HttpStatusCode.NotFound)
    {
    }
}