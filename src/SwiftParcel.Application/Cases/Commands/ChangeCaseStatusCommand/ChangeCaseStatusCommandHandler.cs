using MediatR;
using SwiftParcel.Application.Cases.Events;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.ChangeCaseStatusCommand;

public class ChangeCaseStatusCommandHandler : IRequestHandler<ChangeCaseStatusCommand, Result<ChangeStatusResponse>>

{
    private readonly ICaseRepository _caseRepository;
    private readonly IPublisher _publisher;

    public ChangeCaseStatusCommandHandler(ICaseRepository caseRepository, IPublisher publisher)
    {
        _caseRepository = caseRepository;
        _publisher = publisher;
    }

    public async Task<Result<ChangeStatusResponse>> Handle(ChangeCaseStatusCommand request, CancellationToken cancellationToken)
    {
        var @case = await _caseRepository.GetByCaseNumberWithCustomerAsync(request.CaseNumber, cancellationToken);

        if (@case is null)
        {
            return Result<ChangeStatusResponse>.Failure(Error.NotFound($"Case with number {request.CaseNumber} was not found."));
        }

        var statusResult = @case.ChangeStatus(request.NewStatus);
        if (!statusResult.IsSuccess)
        {
            return Result<ChangeStatusResponse>.Failure(Error.Validation($"Cannot transition case status from {@case.Status} to {request.NewStatus}."));
        }

        @case.Status = request.NewStatus;
        @case.UpdatedDate = DateTime.UtcNow;

        if (request.NewStatus == CaseStatus.Resolved)
        {
            @case.ResolvedDate = DateTime.UtcNow;
        }

        await _caseRepository.UpdateAsync(@case, cancellationToken);

        await _publisher.Publish(new CaseStatusChangedEvent(
            @case.CaseNumber,
            @case.Customer.Email,
            @case.Status
        ), cancellationToken);

        var response = new ChangeStatusResponse(@case.Status, @case.UpdatedDate);
        
        return Result<ChangeStatusResponse>.Success(response);
    }
}