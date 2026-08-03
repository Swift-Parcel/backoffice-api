using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO;

public record UpdateSlaRuleRequest(
    CaseType? CaseType,
    Priority? Priority,
    ServiceType? ServiceType,
    int SlaHours,
    bool IsBusinessHours,
    int? EscalationAfter,
    int? EscalationHandlerId,
    string? EscalationDepartment,
    string? Notes
);

public record SlaRuleResponse(
    int Id,
    string Name,
    CaseType? CaseType,
    Priority? Priority,
    ServiceType? ServiceType,
    int SlaHours,
    bool IsBusinessHours,
    int? EscalationAfter,
    int? EscalationHandlerId,
    string? EscalationDepartment,
    bool IsActive,
    DateTime CreatedDate,
    string? Notes
);