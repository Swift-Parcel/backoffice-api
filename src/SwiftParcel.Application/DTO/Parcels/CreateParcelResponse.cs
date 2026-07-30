using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Parcels;

public record CreateParcelResponse(
    [property: JsonPropertyName("tracking_number")] string TrackingNumber,
    [property: JsonPropertyName("parcel_status")] ParcelStatus ParcelStatus
);