using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Webhooks;

public record DeliveryChangeOutcomeWebhookRequestDto(
    [property: JsonPropertyName("case_number")] string CaseNumber,
    [property: JsonPropertyName("delivery_change_request_outcome")] DeliveryChangeOutcome Outcome
);