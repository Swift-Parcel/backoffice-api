using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Infrastructure.Persistence.Repositories;

public class RegionRepository : IRegionRepository
{
    private readonly AppDbContext _context;

    public RegionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetActiveRegionIdByCountryCodeAsync(string countryCode, CancellationToken cancellationToken = default)
    {
        return await _context.Regions
            .Where(r => r.CountryCode == countryCode && r.IsActive)
            .Select(r => r.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsActiveAsync(int regionId, CancellationToken cancellationToken = default)
    {
        return await _context.Regions
            .AnyAsync(r => r.Id == regionId && r.IsActive, cancellationToken);
    }

    public async Task<List<Region>> GetByIdsAsync(IEnumerable<int> regionIds, CancellationToken cancellationToken = default)
    {
        return await _context.Regions
            .Where(r => regionIds.Contains(r.Id))
            .ToListAsync(cancellationToken);
    }
}