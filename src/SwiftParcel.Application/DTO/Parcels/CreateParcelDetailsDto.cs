using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Parcels;

public record CreateParcelDetailsDto(
    [property: JsonPropertyName("weight")] float Weight,
    [property: JsonPropertyName("height")] float Height,
    [property: JsonPropertyName("width")] float Width,
    [property: JsonPropertyName("length")] float Length,
    [property: JsonPropertyName("service_type")] ServiceType ServiceType,
    [property: JsonPropertyName("declared_value")] float DeclaredValue,
    [property: JsonPropertyName("preferred_pickup_date")] DateTime PreferredPickupDate,
    [property: JsonPropertyName("preferred_pickup_timeslot")] Timeslot PreferredPickupTimeslot
);