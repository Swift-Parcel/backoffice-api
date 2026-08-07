using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.UpdateCaseStatusCommand;

public class UpdateCaseStatusCommandHandler : IRequestHandler<UpdateCaseStatusCommand, Result<Unit>>
{
    private readonly IAppDbContext _context;
    private readonly IWebhookClient _webhookClient;

    public UpdateCaseStatusCommandHandler(IAppDbContext context, IWebhookClient webhookClient)
    {
        _context = context;
        _webhookClient = webhookClient;
    }

    public async Task<Result<Unit>> Handle(Commands.UpdateCaseStatusCommand.UpdateCaseStatusCommand request, CancellationToken cancellationToken)
    {
        var @case = await _context.Cases
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.CaseNumber == request.CaseNumber, cancellationToken);

        // Business Error: Not Found
        if (@case is null)
        {
            return Result<Unit>.Failure(new Error("Case.NotFound", $"Case with number {request.CaseNumber} was not found.", ErrorType.NotFound));
        }

        // Business Error: Invalid Lifecycle Transition
        if (!IsValidStatusTransition(@case.Status, request.NewStatus))
        {
            return Result<Unit>.Failure(new Error("Case.InvalidStatusTransition", 
                $"Cannot transition case status from {@case.Status} to {request.NewStatus}.", 
                ErrorType.Validation));
        }

        @case.Status = request.NewStatus;
        @case.UpdatedDate = DateTime.UtcNow;

        if (request.NewStatus == CaseStatus.Resolved)
        {
            @case.ResolvedDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _webhookClient.NotifyCaseStatusChangedAsync(
            @case.CaseNumber,
            @case.Customer.Email,
            @case.Status,
            cancellationToken
        );

        return Result<Unit>.Success(Unit.Value);
    }

    private static bool IsValidStatusTransition(CaseStatus currentStatus, CaseStatus newStatus)
    {
        if (currentStatus == newStatus) return true;

        return currentStatus switch
        {
            CaseStatus.Open => newStatus
                is CaseStatus.InProgress 
                or CaseStatus.Escalated 
                or CaseStatus.Cancelled,

            CaseStatus.InProgress => newStatus
                is CaseStatus.AwaitingCustomer 
                or CaseStatus.Resolved 
                or CaseStatus.Escalated 
                or CaseStatus.Cancelled,

            CaseStatus.AwaitingCustomer => newStatus
                is CaseStatus.InProgress 
                or CaseStatus.Resolved 
                or CaseStatus.Cancelled,

            CaseStatus.Escalated => newStatus
                is CaseStatus.InProgress 
                or CaseStatus.Resolved 
                or CaseStatus.Cancelled,

            CaseStatus.Resolved => newStatus
                is CaseStatus.Closed 
                or CaseStatus.InProgress,

            CaseStatus.Closed => false,
            CaseStatus.Cancelled => false,

            _ => false
        };
    }
}