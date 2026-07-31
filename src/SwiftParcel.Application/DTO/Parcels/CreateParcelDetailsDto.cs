using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Parcels;

public record CreateParcelDetailsDto(
     float Weight,
     int Height,
     int Width,
     int Length,
     ServiceType ServiceType,
     float DeclaredValue,
     DateTime PreferredPickupDate,
     Timeslot PreferredPickupTimeslot
);