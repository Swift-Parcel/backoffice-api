using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Parcels;

public record TrackingHistoryDto(
     DateTime Timestamp,
     ParcelStatus ParcelStatus,
     string Description,
     LocationDto Location
);