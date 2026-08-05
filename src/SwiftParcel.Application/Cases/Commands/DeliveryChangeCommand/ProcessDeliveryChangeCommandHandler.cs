using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.Integration.Interfaces;

namespace SwiftParcel.Application.Cases.Commands.ProcessDeliveryChange;

public class ProcessDeliveryChangeCommandHandler : IRequestHandler<ProcessDeliveryChangeCommand, Result<Unit>>
{
    private readonly IAppDbContext _context;
    private readonly IWebhookClient _webhookClient;

    public ProcessDeliveryChangeCommandHandler(IAppDbContext context, IWebhookClient webhookClient)
    {
        _context = context;
        _webhookClient = webhookClient;
    }

    public async Task<Result<Unit>> Handle(ProcessDeliveryChangeCommand request, CancellationToken cancellationToken)
    {
        var @case = await _context.Cases
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.CaseNumber == request.CaseNumber, cancellationToken);

        // Business Error: Not Found
        if (@case is null)
        {
            return Result<Unit>.Failure(new Error(
                "Case.NotFound", 
                $"Case with number {request.CaseNumber} was not found.", 
                ErrorType.NotFound));
        }

        @case.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        await _webhookClient.NotifyDeliveryChangeOutcomeAsync(
            @case.CaseNumber,
            @case.Customer.Email,
            request.Outcome,
            cancellationToken
        );

        return Result<Unit>.Success(Unit.Value);
    }
}