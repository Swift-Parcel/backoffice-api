using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Services;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Infrastructure.Services;

public class RegionRoutingService : IRegionRoutingService
{
    private readonly IAppDbContext _context;
    private const int DefaultCentralHubId = 1; // Fallback region ID
    private readonly IParcelInformationService _parcelInformationService;

    public RegionRoutingService(IAppDbContext context, IParcelInformationService parcelInformationService)
    {
        _context = context;
        _parcelInformationService = parcelInformationService;
    }

    public async Task<int> DetermineRegionAsync(Case caseEntity, CancellationToken cancellationToken = default)
    {
        string? countryCode = caseEntity.Parcels.FirstOrDefault()?.RecipientAddress.CountryCode
                              ?? caseEntity.Customer?.Address.CountryCode;

        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return DefaultCentralHubId;
        }

        var availableRegionIds = await _context.Regions
            .AsNoTracking()
            .Where(r => r.CountryCode == countryCode && r.IsActive)
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        if (availableRegionIds.Count == 0)
        {
            return DefaultCentralHubId;
        }

        int randomIndex = Random.Shared.Next(availableRegionIds.Count);
        return availableRegionIds[randomIndex];
    }

    public async Task<int> DetermineRegionAsync(IEnumerable<Parcel> parcels,
        CancellationToken cancellationToken = default)
    {
        if (parcels == null || !parcels.Any())
        {
            throw new ArgumentException("At least one parcel needs to be provided.", nameof(parcels));
        }
        
        return await CalculateRegionFromParcelsAsync(parcels, cancellationToken);
    }
    
    public Task<int> DetermineRegionAsync(
        Parcel parcel, 
        CancellationToken cancellationToken = default)
    {
        if (parcel == null) throw new ArgumentNullException(nameof(parcel));

        return DetermineRegionAsync([parcel], cancellationToken);
    }
    private async Task<int> CalculateRegionFromParcelsAsync(IEnumerable<Parcel> parcels
        , CancellationToken cancellationToken)
    {
        var firstParcel = parcels.First();
        var firstParcelLocation = await _parcelInformationService
            .GetLocationByTrackingNumberAsync(firstParcel.TrackingNumber, cancellationToken);

        if (firstParcelLocation == null)
        {
            return DefaultCentralHubId;
        }
        
        var countryCode = firstParcelLocation.CountryCode;
        
        var availableRegionIds = await _context.Regions
            .AsNoTracking()
            .Where(r => r.CountryCode == countryCode && r.IsActive)
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        if (availableRegionIds.Count == 0)
        {
            return DefaultCentralHubId;
        }

        int randomIndex = Random.Shared.Next(availableRegionIds.Count);
        return availableRegionIds[randomIndex];
    }
}