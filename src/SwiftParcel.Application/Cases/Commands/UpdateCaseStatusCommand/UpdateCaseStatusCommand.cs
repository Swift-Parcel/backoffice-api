using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.UpdateCaseStatusCommand;

public record UpdateCaseStatusCommand(string CaseNumber, CaseStatus NewStatus) : IRequest<Result<Unit>>;