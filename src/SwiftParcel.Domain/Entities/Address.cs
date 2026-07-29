namespace SwiftParcel.Domain.Entities;

public class Address
{
    public int Id { get; set; }
    public string Street { get; set; } = string.Empty;
    public string StreetNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;
    public Country Country { get; set; } = null!;

    public ICollection<Parcel> Parcels { get; set; } = new List<Parcel>();
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
}