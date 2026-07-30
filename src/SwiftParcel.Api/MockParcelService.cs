using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.Integration.Models;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Api;

public class MockParcelService : IParcelIntegrationService
{
    public Task<ParcelTrackingResponse?> GetParcelTrackingAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        if (trackingNumber == "999") 
        {
            return Task.FromResult<ParcelTrackingResponse?>(null);
        }

        var response = new ParcelTrackingResponse(
            ParcelStatus.Lost,
            new LocationDto("Central Hub", "Budapest", "HU", "1000", 47.4979, 19.0402),
            new List<TrackingHistoryDto>
            {
                new TrackingHistoryDto(DateTime.UtcNow.AddDays(-1), ParcelStatus.InTransit, "Label created", new LocationDto(null, "Pétervására", "HU", "3250", 48.0167, 20.1000)),
                new TrackingHistoryDto(DateTime.UtcNow, ParcelStatus.InTransit, "Arrived at hub", new LocationDto("Central Hub", "Budapest", "HU", "1000", 47.4979, 19.0402))
            }
        );

        return Task.FromResult<ParcelTrackingResponse?>(response);
    }

    public Task<DeliveryEstimateResponse?> GetDeliveryEstimateAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CustomerParcelDto?> GetCustomerParcelAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CreateParcelRequest?> GetCreateParcelRequestAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CreateParcelResponse?> CreateParcelAsync(CreateParcelRequest createParcelRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<DeliveryChangeRequest?> GetDeliveryChangeRequestAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<DeliveryEstimateResponse?> CreateDeliveryEstimateAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ConfirmDeliveryRequest?> GetConfirmDeliveryRequestAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ParcelStatusResponse?> GetParcelStatusAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        if (trackingNumber == "999") 
        {
            return Task.FromResult<ParcelStatusResponse?>(null);
        }

        var response = new ParcelStatusResponse(
            ParcelStatus.DeliveryAttemptFailed
        );
        
        return Task.FromResult<ParcelStatusResponse?>(response);
    }
}