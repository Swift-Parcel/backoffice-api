using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Integration.Models;

public record TrackingHistoryDto(
    [property: JsonPropertyName("timestamp")] DateTime Timestamp,
    [property: JsonPropertyName("parcel_status")] ParcelStatus ParcelStatus,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("location")] LocationDto Location
);