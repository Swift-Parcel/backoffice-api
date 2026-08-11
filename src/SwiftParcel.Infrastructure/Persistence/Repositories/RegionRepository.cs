using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces.Repositories;

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
    
    public async Task<(List<Region> Items, int TotalCount)> GetPagedAsync(
        string? nameFilter, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Regions
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            var term = nameFilter.ToLower();
            query = query.Where(r => r.Name.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(r => r.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}