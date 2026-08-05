using MediatR;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Cases.Commands.AssignCase;

public record AssignCaseCommand(string CaseNumber, int HandlerId) : IRequest<Result<Unit>>;