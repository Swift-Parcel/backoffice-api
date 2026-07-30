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
    
    public Task<ParcelTrackingResponse?> GetParcelTrackingAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ParcelStatusResponse?> GetParcelStatusAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}