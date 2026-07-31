using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Webhooks;

public record CaseStatusWebhookRequestDto(
     string CaseNumber,
     CaseStatus CaseStatus
);