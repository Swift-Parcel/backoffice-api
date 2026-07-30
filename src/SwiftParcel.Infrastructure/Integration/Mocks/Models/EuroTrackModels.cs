using System.Text.Json.Serialization;

namespace SwiftParcel.Infrastructure.Integration.Models;

public record EuroTrackResponseDto(
    [property: JsonPropertyName("shipments")] List<EuroTrackShipmentDto> Shipments
);

public record EuroTrackShipmentDto(
    [property: JsonPropertyName("trackingNumber")] string TrackingNumber,
    [property: JsonPropertyName("currentStatus")] string CurrentStatus,
    [property: JsonPropertyName("events")] List<EuroTrackEventDto> Events
);

public record EuroTrackEventDto(
    [property: JsonPropertyName("timestamp")] DateTime Timestamp,
    [property: JsonPropertyName("statusCode")] string StatusCode,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("location")] EuroTrackLocation Location
);

public record EuroTrackLocation(
    [property: JsonPropertyName("facility")] string Facility,
    [property: JsonPropertyName("city")] string City,
    [property: JsonPropertyName("countryCode")] string CountryCode,
    [property: JsonPropertyName("postalCode")] string PostalCode,
    [property: JsonPropertyName("lat")] double? Lat,
    [property: JsonPropertyName("lon")] double? Lon
);