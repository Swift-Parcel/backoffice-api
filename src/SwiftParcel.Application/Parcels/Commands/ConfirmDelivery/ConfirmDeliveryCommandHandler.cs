using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Parcels.Commands.ConfirmDelivery;

public class ConfirmDeliveryCommandHandler : IRequestHandler<ConfirmDeliveryCommand, Result<bool>>
{
    private readonly IAppDbContext _context;
    private readonly IWebhookClient _webhookClient;

    public ConfirmDeliveryCommandHandler(IAppDbContext context, IWebhookClient webhookClient)
    {
        _context = context;
        _webhookClient = webhookClient;
    }

    public async Task<Result<bool>> Handle(ConfirmDeliveryCommand request, CancellationToken cancellationToken)
    {
        var parcel = await _context.Parcels
            .FirstOrDefaultAsync(p => p.TrackingNumber == request.TrackingNumber, cancellationToken);

        if (parcel == null)
        {
            return Result<bool>.Failure(
                Error.NotFound("parcel_not_found", $"Parcel '{request.TrackingNumber}' not found."));
        }

        parcel.Status = ParcelStatus.Delivered;
        parcel.DeliveredDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        await _webhookClient.NotifyParcelStatusChangedAsync(parcel.TrackingNumber, parcel.Status, cancellationToken);

        return Result<bool>.Success(true);
    }
}