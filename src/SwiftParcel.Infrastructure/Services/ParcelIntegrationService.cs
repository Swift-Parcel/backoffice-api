using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.Integration.Models;
using SwiftParcel.Infrastructure.Persistence;

namespace SwiftParcel.Infrastructure.Services;

public class ParcelIntegrationService : IParcelIntegrationService
{
    private readonly AppDbContext _dbContext;

    public ParcelIntegrationService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ParcelStatusResponse?> GetParcelStatusAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ParcelTrackingResponse?> GetParcelTrackingAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
}