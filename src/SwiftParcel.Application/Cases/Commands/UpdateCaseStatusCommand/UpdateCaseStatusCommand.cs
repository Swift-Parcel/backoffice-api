using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Commands.UpdateCaseStatus;

public record UpdateCaseStatusCommand(string CaseNumber, CaseStatus NewStatus) : IRequest<Result<Unit>>;