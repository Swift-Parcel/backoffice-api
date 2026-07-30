using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Integration.Models;

public record ParcelTrackingResponse(
    [property: JsonPropertyName("parcel_status")] ParcelStatus ParcelStatus,
    [property: JsonPropertyName("location")] LocationDto Location,
    [property: JsonPropertyName("tracking_history")] List<TrackingHistoryDto> TrackingHistory
);

