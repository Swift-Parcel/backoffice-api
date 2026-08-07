using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Parcels;

public record DeliveryChangeRequest(
     DateTime? Date = null,
     Timeslot? Timeslot = null
);