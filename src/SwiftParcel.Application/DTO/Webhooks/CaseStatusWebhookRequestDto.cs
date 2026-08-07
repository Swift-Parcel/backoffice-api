using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Webhooks;

public record CaseStatusWebhookRequestDto(
     string Email,
     string CaseNumber,
     CaseStatus CaseStatus
);