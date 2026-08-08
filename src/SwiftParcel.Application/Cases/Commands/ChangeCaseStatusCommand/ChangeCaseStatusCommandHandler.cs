using MediatR;
using SwiftParcel.Application.Cases.Commands.UpdateCaseStatusCommand;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.ChangeCaseStatusCommand;

public class ChangeCaseStatusCommandHandler : IRequestHandler<ChangeStatusCommand, Result<Unit>>
{
    private readonly ICaseRepository _caseRepository;
    private readonly IWebhookClient _webhookClient;

    public ChangeCaseStatusCommandHandler(ICaseRepository caseRepository, IWebhookClient webhookClient)
    {
        _caseRepository = caseRepository;
        _webhookClient = webhookClient;
    }

    public async Task<Result<Unit>> Handle(ChangeStatusCommand request, CancellationToken cancellationToken)
    {
        var @case = await _caseRepository.GetByCaseNumberWithCustomerAsync(request.CaseNumber, cancellationToken);

        if (@case is null)
        {
            return Result<Unit>.Failure(Error.NotFound($"Case with number {request.CaseNumber} was not found."));
        }

        if (!IsValidStatusTransition(@case.Status, request.NewStatus))
        {
            return Result<Unit>.Failure(Error.Validation($"Cannot transition case status from {@case.Status} to {request.NewStatus}."));
        }

        @case.Status = request.NewStatus;
        @case.UpdatedDate = DateTime.UtcNow;

        if (request.NewStatus == CaseStatus.Resolved)
        {
            @case.ResolvedDate = DateTime.UtcNow;
        }

        await _caseRepository.UpdateAsync(@case, cancellationToken);

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