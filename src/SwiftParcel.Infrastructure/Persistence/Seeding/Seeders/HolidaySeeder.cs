namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;

public class HolidaySeeder : BaseCsvRelationSeeder<Holiday, Region>
{
    private List<Region> _allRegions = new();

    public override int Order => 10;
    protected override string SqlQuery => "SELECT id, region FROM holidays WHERE region IS NOT NULL AND region != ''";

    protected override async Task<Dictionary<int, Holiday>> GetEntitiesAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        _allRegions = await dbContext.Regions.ToListAsync(cancellationToken);
        return await dbContext.Holidays.Include(h => h.Regions).ToDictionaryAsync(h => h.Id, cancellationToken);
    }

    protected override Task<List<Region>> ResolveTargetsAsync(AppDbContext dbContext, string token, CancellationToken cancellationToken)
    {
        if (token.Equals("ALL", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(_allRegions);
        }

        var found = _allRegions.Where(r =>
            string.Equals(r.CountryCode, token, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(r.RegionName, token, StringComparison.OrdinalIgnoreCase)).ToList();

        return Task.FromResult(found);
    }

    protected override bool RelationExists(Holiday entity, Region target) => entity.Regions.Any(r => r.Id == target.Id);
    protected override void AttachRelation(Holiday entity, Region target) => entity.Regions.Add(target);
}