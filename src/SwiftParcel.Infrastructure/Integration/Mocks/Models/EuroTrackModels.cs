using System.Text.Json.Serialization;

namespace SwiftParcel.Infrastructure.Integration.Models;

public record EuroTrackResponse(
    [property: JsonPropertyName("shipments")] List<EuroTrackShipment> Shipments
);

public record EuroTrackShipment(
    [property: JsonPropertyName("trackingNumber")] string TrackingNumber,
    [property: JsonPropertyName("events")] List<EuroTrackEvent> Events
);

public record EuroTrackEvent(
    [property: JsonPropertyName("timestamp")] DateTime Timestamp,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("location")] EuroTrackLocation Location
);

public record EuroTrackLocation(
    [property: JsonPropertyName("facility")] string Facility,
    [property: JsonPropertyName("city")] string City,
    [property: JsonPropertyName("countryCode")] string CountryCode,
    [property: JsonPropertyName("lat")] double? Lat,
    [property: JsonPropertyName("lon")] double? Lon
);