using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Parcels;

public record CreateParcelResponse(
     string TrackingNumber,
     ParcelStatus ParcelStatus
);