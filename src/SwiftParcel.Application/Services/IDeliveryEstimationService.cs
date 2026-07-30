using SwiftParcel.Application.DTO.Parcels;

namespace SwiftParcel.Application.Services;

public interface IDeliveryEstimationService
{
    Task<DeliveryEstimateResponse> CalculateForParcelAsync(string trackingNumber, CancellationToken cancellationToken = default);
}