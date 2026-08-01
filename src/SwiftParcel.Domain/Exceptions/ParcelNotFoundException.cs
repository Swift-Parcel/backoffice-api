namespace SwiftParcel.Domain.Exceptions;

public class ParcelNotFoundException : DomainException
{
    public ParcelNotFoundException(string trackingNumber)
        : base("parcel_not_found", 
            $"Parcel with tracking number '{trackingNumber}' was not found.", 
            404)
    {
    }
}