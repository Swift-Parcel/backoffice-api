using MediatR;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Cases.Commands.AssignCase;

public record AssignCaseCommand(int CaseId, int HandlerId) : IRequest<Result<Unit>>;