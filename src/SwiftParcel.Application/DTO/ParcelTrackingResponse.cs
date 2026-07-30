using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Integration.Models;

public record ParcelTrackingResponse(
    [property: JsonPropertyName("parcel_status")] ParcelStatus ParcelStatus,
    [property: JsonPropertyName("location")] LocationDto Location,
    [property: JsonPropertyName("tracking_history")] List<TrackingHistoryDto> TrackingHistory
);

public record LocationDto(
    [property: JsonPropertyName("facility")] string? Facility,
    [property: JsonPropertyName("city")] string City,
    [property: JsonPropertyName("country_code")] string CountryCode,
    [property: JsonPropertyName("postal_code")] string PostalCode,
    [property: JsonPropertyName("lat")] double Lat,
    [property: JsonPropertyName("lon")] double Lon
);

public record TrackingHistoryDto(
    [property: JsonPropertyName("timestamp")] DateTime Timestamp,
    [property: JsonPropertyName("parcel_status")] ParcelStatus ParcelStatus,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("location")] LocationDto Location
);