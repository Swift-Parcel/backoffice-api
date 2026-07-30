using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Parcels;

public record DeliveryChangeRequest(
    [property: JsonPropertyName("date")] DateTime? Date,
    [property: JsonPropertyName("timeslot")] Timeslot? Timeslot
);