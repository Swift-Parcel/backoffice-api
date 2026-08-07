using MediatR;
using SwiftParcel.Application.Cases.Dtos;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Cases.Commands.AssignCase;

public class AssignCaseCommandHandler(ICaseAssignmentService assignmentService)
    : IRequestHandler<AssignCaseCommand, Result<CaseSummaryDto>>
{
    public async Task<Result<CaseSummaryDto>> Handle(AssignCaseCommand request, CancellationToken cancellationToken)
    {
        return await assignmentService
            .AssignCaseAsync(request.CaseNumber, request.HandlerId, cancellationToken);
    }
}