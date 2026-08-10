using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Commands.ChangeCaseStatusCommand;

public record ChangeStatusResponse(CaseStatus NewStatus, DateTime? UpdatedDate);