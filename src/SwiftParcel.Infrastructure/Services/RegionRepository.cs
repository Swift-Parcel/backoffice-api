using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence;

namespace SwiftParcel.Infrastructure.Services;

public class RegionRepository(AppDbContext dbContext) : IRegionRepository
{
    public async Task<int> GetActiveRegionIdByCountryCodeAsync(string countryCode, CancellationToken cancellationToken = default)
    {
        return await dbContext.Regions
            .Where(r => r.CountryCode == countryCode && r.IsActive)
            .Select(r => r.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Region>> GetByIdsAsync(IEnumerable<int> regionIds, CancellationToken cancellationToken = default)
    {
        return await dbContext.Regions
            .Where(r => regionIds.Contains(r.Id))
            .ToListAsync(cancellationToken);
    }
}