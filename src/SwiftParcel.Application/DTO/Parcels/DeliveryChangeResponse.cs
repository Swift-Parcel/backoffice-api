using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO.Parcels;

public record DeliveryChangeResponse(
    [property: JsonPropertyName("case_number")] string CaseNumber
);