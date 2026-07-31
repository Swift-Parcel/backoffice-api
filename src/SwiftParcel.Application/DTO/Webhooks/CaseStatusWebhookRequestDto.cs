using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Webhooks;

public record CaseStatusWebhookRequestDto(
    [property: JsonPropertyName("case_number")] string CaseNumber,
    [property: JsonPropertyName("case_status")] CaseStatus CaseStatus
);