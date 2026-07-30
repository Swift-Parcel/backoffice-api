using System.Text.Json.Serialization;

namespace SwiftParcel.Application.Integration.Models;

public record AddressDto(
    [property: JsonPropertyName("city")] string City,
    [property: JsonPropertyName("country_code")] string CountryCode,
    [property: JsonPropertyName("postal_code")] string PostalCode,
    [property: JsonPropertyName("street")] string Street,
    [property: JsonPropertyName("street_number")] string StreetNumber
);