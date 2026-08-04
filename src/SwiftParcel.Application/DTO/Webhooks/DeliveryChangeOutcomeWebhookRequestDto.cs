using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Webhooks;

public record DeliveryChangeOutcomeWebhookRequestDto(
     string Email,
     string CaseNumber,
     DeliveryChangeOutcome Outcome
);