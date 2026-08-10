using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Dtos;

public record CreateCaseResponse(
    string CaseNumber,
    CaseStatus Status,
    Priority Priority,
    DateTime CreatedDate
);