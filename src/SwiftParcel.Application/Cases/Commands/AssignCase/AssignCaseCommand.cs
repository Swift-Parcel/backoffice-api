using MediatR;
using SwiftParcel.Application.Cases.Dtos;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Cases.Commands.AssignCase;

public record AssignCaseCommand(string CaseNumber, int HandlerId) : IRequest<Result<CaseSummaryDto>>;