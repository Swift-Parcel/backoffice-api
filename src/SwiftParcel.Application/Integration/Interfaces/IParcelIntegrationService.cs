using SwiftParcel.Application.Integration.Models;

namespace SwiftParcel.Application.Integration.Interfaces;

public interface IParcelIntegrationService
{
    Task<ParcelTrackingResponse?> GetParcelTrackingAsync(string trackingNumber, CancellationToken cancellationToken = default);
    Task<ParcelStatusResponse?> GetParcelStatusAsync(string trackingNumber, CancellationToken cancellationToken = default);
}