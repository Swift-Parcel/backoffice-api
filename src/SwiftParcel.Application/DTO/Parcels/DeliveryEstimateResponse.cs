using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Parcels;

public record DeliveryEstimateResponse(
    [property: JsonPropertyName("estimated_delivery")] DateTime? EstimatedDelivery,
    [property: JsonPropertyName("time_slots")] List<Timeslot> TimeSlots
);