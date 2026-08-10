using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Dtos;

public record CreateCaseResponse(
    string CaseNumber,
    Priority Priority,
    DateTime CreatedDate
);