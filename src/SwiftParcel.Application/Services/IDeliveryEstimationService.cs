using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Domain.ValueObjects;

namespace SwiftParcel.Application.Services;

public interface IDeliveryEstimationService
{
    Task<DeliveryEstimateResponse> CalculateForParcelAsync(TrackingNumber trackingNumber, CancellationToken cancellationToken = default);
}