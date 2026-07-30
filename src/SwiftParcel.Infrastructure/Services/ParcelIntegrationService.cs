using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.Integration.Models;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Infrastructure.Persistence;

namespace SwiftParcel.Infrastructure.Services;

public class ParcelIntegrationService : IParcelIntegrationService
{
    private readonly AppDbContext _dbContext;

    public ParcelIntegrationService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ParcelStatusResponse?> GetParcelStatusAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        var response = await _dbContext.Parcels
            .Where(p => p.TrackingNumber == trackingNumber)
            .Select(p => new ParcelStatusResponse(p.Status))
            .FirstOrDefaultAsync(cancellationToken);

        return response;
    }

    public async Task<ParcelTrackingResponse?> GetParcelTrackingAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<DeliveryEstimateResponse?> GetDeliveryEstimateAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CustomerParcelDto?> GetCustomerParcelAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CreateParcelRequest?> GetCreateParcelRequestAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CreateParcelResponse?> CreateParcelAsync(CreateParcelRequest createParcelRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<DeliveryChangeRequest?> GetDeliveryChangeRequestAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<DeliveryEstimateResponse?> CreateDeliveryEstimateAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ConfirmDeliveryRequest?> GetConfirmDeliveryRequestAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}