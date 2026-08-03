using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.SlaRules.Commands.UpdateSlaRule;

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

public record UpdateSlaRuleCommand(
    int Id,
    CaseType? CaseType,
    Priority? Priority,
    ServiceType? ServiceType,
    int SlaHours,
    bool IsBusinessHours,
    int? EscalationAfter,
    int? EscalationHandlerId,
    string? EscalationDepartment,
    string? Notes
) : IRequest<Result<SlaRuleResponse>>;