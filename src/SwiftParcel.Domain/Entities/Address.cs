namespace SwiftParcel.Domain.Entities;

public record Address(
    string Street,
    string StreetNumber,
    string City,
    string PostalCode,
    string CountryCode);