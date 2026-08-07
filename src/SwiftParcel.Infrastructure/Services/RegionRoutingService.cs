using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Services;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Infrastructure.Services;

public class RegionRoutingService : IRegionRoutingService
{
    private readonly IAppDbContext _context; 
    private const int DefaultCentralHubId = 1; // Fallback region ID

    public RegionRoutingService(IAppDbContext context)
    {
        _context = context;
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
}