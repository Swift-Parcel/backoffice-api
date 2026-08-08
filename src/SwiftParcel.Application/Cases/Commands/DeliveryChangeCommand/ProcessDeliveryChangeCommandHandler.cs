using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.DeliveryChangeCommand;

public class ProcessDeliveryChangeCommandHandler : IRequestHandler<ProcessDeliveryChangeCommand, Result<Unit>>
{
    private readonly ICaseRepository _caseRepository;
    private readonly IWebhookClient _webhookClient;

    public ProcessDeliveryChangeCommandHandler(ICaseRepository caseRepository, IWebhookClient webhookClient)
    {
        _caseRepository = caseRepository;
        _webhookClient = webhookClient;
    }

    public async Task<Result<Unit>> Handle(ProcessDeliveryChangeCommand request, CancellationToken cancellationToken)
    {
        var @case = await _caseRepository.GetByCaseNumberWithCustomerAsync(request.CaseNumber, cancellationToken);

        if (@case is null)
        {
            return Result<Unit>.Failure(Error.NotFound($"Case with number {request.CaseNumber} was not found."));
        }

        @case.UpdatedDate = DateTime.UtcNow;

        await _caseRepository.UpdateAsync(@case, cancellationToken);

        await _webhookClient.NotifyDeliveryChangeOutcomeAsync(
            @case.CaseNumber,
            @case.Customer.Email,
            request.Outcome,
            cancellationToken
        );

        return Result<Unit>.Success(Unit.Value);
    }
}