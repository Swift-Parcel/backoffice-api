using MediatR;
using Microsoft.Extensions.Logging;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Parcels.Commands.ConfirmDelivery;

public class ConfirmDeliveryCommandHandler : IRequestHandler<ConfirmDeliveryCommand, Result<bool>>
{
    private readonly IParcelRepository _parcelRepository;
    private readonly IWebhookClient _webhookClient;
    private readonly ILogger<ConfirmDeliveryCommandHandler> _logger;

    public ConfirmDeliveryCommandHandler(
        IParcelRepository parcelRepository, 
        IWebhookClient webhookClient, 
        ILogger<ConfirmDeliveryCommandHandler> logger)
    {
        _parcelRepository = parcelRepository;
        _webhookClient = webhookClient;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(ConfirmDeliveryCommand request, CancellationToken cancellationToken)
    {
        var parcel = await _parcelRepository.GetByTrackingNumberAsync(request.TrackingNumber, cancellationToken);

        if (parcel == null)
        {
            return Result<bool>.Failure(
                Error.NotFound("parcel_not_found", $"Parcel '{request.TrackingNumber}' not found."));
        }

        if (parcel.Customer == null || !string.Equals(parcel.Customer.Email, request.CustomerEmail, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Delivery confirmation failed for Parcel {TrackingNumber}: Email mismatch.", parcel.TrackingNumber);
            
            return Result<bool>.Failure(
                Error.Validation("invalid_customer_email", "The provided email does not match the recipient on record."));
        }

        parcel.Status = ParcelStatus.Delivered;
        parcel.DeliveredDate = DateTime.UtcNow;

        await _parcelRepository.UpdateAsync(parcel, cancellationToken);

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