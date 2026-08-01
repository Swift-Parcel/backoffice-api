using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Parcels;

public record ParcelTrackingResponse(
     ParcelStatus ParcelStatus,
     LocationDto Location,
     List<TrackingHistoryDto> TrackingHistory
);

