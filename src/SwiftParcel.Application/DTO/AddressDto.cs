namespace SwiftParcel.Application.DTO;

public record AddressDto(
     string City,
     string CountryCode,
     string PostalCode,
     string Street,
     string StreetNumber
);