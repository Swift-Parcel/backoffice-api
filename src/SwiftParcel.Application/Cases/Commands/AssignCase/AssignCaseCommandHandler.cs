using MediatR;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Cases.Commands.AssignCase;

public class AssignCaseCommandHandler(ICaseAssignmentService assignmentService)
    : IRequestHandler<AssignCaseCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(AssignCaseCommand request, CancellationToken cancellationToken)
    {
        // The service handles transaction locking and capacity enforcement.
        // It will throw DomainExceptions if business rules are violated.
        await assignmentService.AssignCaseAsync(request.CaseId, request.HandlerId, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}