using System.Text.Json.Serialization;

namespace SwiftParcel.Application.Integration.Models;

public record LocationDto(
    [property: JsonPropertyName("facility")] string? Facility,
    [property: JsonPropertyName("city")] string City,
    [property: JsonPropertyName("country_code")] string CountryCode,
    [property: JsonPropertyName("postal_code")] string PostalCode,
    [property: JsonPropertyName("lat")] double Lat,
    [property: JsonPropertyName("lon")] double Lon
);