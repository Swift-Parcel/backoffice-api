using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Parcels;

public record DeliveryEstimateResponse(
     DateTime? EstimatedDelivery,
     List<Timeslot> TimeSlots
);