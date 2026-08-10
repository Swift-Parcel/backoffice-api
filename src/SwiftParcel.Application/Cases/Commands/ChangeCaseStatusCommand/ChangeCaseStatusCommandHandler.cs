using MediatR;
using SwiftParcel.Application.Cases.Events;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.ChangeCaseStatusCommand;

public class ChangeCaseStatusCommandHandler : IRequestHandler<ChangeCaseStatusCommand, Result<Unit>>
{
    private readonly ICaseRepository _caseRepository;
    private readonly IPublisher _publisher;

    public ChangeCaseStatusCommandHandler(ICaseRepository caseRepository, IPublisher publisher)
    {
        _caseRepository = caseRepository;
        _publisher = publisher;
    }

    public async Task<Result<Unit>> Handle(ChangeCaseStatusCommand request, CancellationToken cancellationToken)
    {
        var @case = await _caseRepository.GetByCaseNumberWithCustomerAsync(request.CaseNumber, cancellationToken);

        if (@case is null)
        {
            return Result<Unit>.Failure(Error.NotFound($"Case with number {request.CaseNumber} was not found."));
        }

        // 1. Státuszváltás és validáció az entitásban
        var statusResult = @case.ChangeStatus(request.NewStatus);
        if (!statusResult.IsSuccess)
        {
            return Result<Unit>.Failure(statusResult.Error);
        }

        // 2. Mentés adatbázisba
        await _caseRepository.UpdateAsync(@case, cancellationToken);

        // 3. Értesítési event publikálása
        await _publisher.Publish(new CaseStatusChangedEvent(
            @case.CaseNumber,
            @case.Customer.Email,
            @case.Status
        ), cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}