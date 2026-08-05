using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Parcels.Commands.ConfirmDelivery;

public class ConfirmDeliveryCommandHandler : IRequestHandler<ConfirmDeliveryCommand, Result<bool>>
{
    private readonly IAppDbContext _context;
    private readonly IWebhookClient _webhookClient;
    private readonly ILogger<ConfirmDeliveryCommandHandler> _logger;

    public ConfirmDeliveryCommandHandler(
        IAppDbContext context, 
        IWebhookClient webhookClient, 
        ILogger<ConfirmDeliveryCommandHandler> logger)
    {
        _context = context;
        _webhookClient = webhookClient;
        _logger = logger;
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

        try
        {
            await _webhookClient.NotifyParcelStatusChangedAsync(parcel.TrackingNumber, parcel.Status, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch webhook notification for Parcel {TrackingNumber} delivery confirmation.", parcel.TrackingNumber);
        }

        return Result<bool>.Success(true);
    }
}