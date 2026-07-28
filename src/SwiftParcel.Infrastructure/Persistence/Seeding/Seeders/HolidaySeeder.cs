namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

using Microsoft.EntityFrameworkCore;
using Interfaces;
using Domain.Entities;
using Helpers;

public class HolidaySeeder : IEntitySeeder
{
    public int Order => 10;

    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        // Load all regions for potential "ALL" wildcard matching
        var allRegions = await dbContext.Regions.ToListAsync(cancellationToken);

        // Load existing relations into a HashSet to prevent duplicate insertions
        var existingHolidayRegions = await dbContext.HolidayRegions
            .Select(hr => new { hr.HolidayId, hr.RegionId })
            .ToHashSetAsync(cancellationToken);

        var holidayRegionsToInsert = new List<HolidayRegion>();

        // Raw SQL query to fetch raw region codes from legacy holidays table
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT id, region FROM holidays WHERE region IS NOT NULL AND region != ''";

        await dbContext.Database.OpenConnectionAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        // Process results row-by-row
        while (await reader.ReadAsync(cancellationToken))
        {
            var holidayId = reader.GetInt32(0);
            var rawRegions = reader.GetString(1);

            // Parse and clean CSV region tokens
            var regionTokens = StringParserHelper.ParseCsvString(rawRegions);

            foreach (var token in regionTokens)
            {
                List<Region> matchedRegions = new();

                // Handle "ALL" keyword vs specific country/region matching
                if (token.Equals("ALL", StringComparison.OrdinalIgnoreCase))
                {
                    matchedRegions = allRegions;
                }
                else
                {
                    var found = allRegions.Where(r =>
                        string.Equals(r.CountryCode, token, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(r.RegionName, token, StringComparison.OrdinalIgnoreCase)).ToList();

                    matchedRegions.AddRange(found);
                }

                foreach (var region in matchedRegions)
                {
                    var pair = new { HolidayId = holidayId, RegionId = region.Id };

                    // Ensure record doesn't exist in the database or in the current batch
                    if (!existingHolidayRegions.Contains(pair) &&
                        !holidayRegionsToInsert.Any(hr => hr.HolidayId == holidayId && hr.RegionId == region.Id))
                    {
                        holidayRegionsToInsert.Add(new HolidayRegion
                        {
                            HolidayId = holidayId,
                            RegionId = region.Id
                        });
                    }
                }
            }
        }

        // Bulk insert mapped entities in a single transaction
        if (holidayRegionsToInsert.Any())
        {
            await dbContext.HolidayRegions.AddRangeAsync(holidayRegionsToInsert, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}