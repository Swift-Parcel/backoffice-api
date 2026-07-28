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

        // Fetch existing Holidays with their loaded Regions to prevent duplicate assignments
        var holidays = await dbContext.Holidays
            .Include(h => h.Regions)
            .ToDictionaryAsync(h => h.Id, cancellationToken);

        // Raw SQL query to fetch raw region codes from legacy holidays table
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT id, region FROM holidays WHERE region IS NOT NULL AND region != ''";

        await dbContext.Database.OpenConnectionAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var hasChanges = false;

        // Process results row-by-row
        while (await reader.ReadAsync(cancellationToken))
        {
            var holidayId = reader.GetInt32(0);
            var rawRegions = reader.GetString(1);

            // Parse and clean CSV region tokens
            var regionTokens = StringParserHelper.ParseCsvString(rawRegions);

            if (holidays.TryGetValue(holidayId, out var currentHoliday))
            {
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
                        // Ensure region is not already linked to the Holiday
                        if (currentHoliday.Regions.All(r => r.Id == region.Id))
                        {
                            currentHoliday.Regions.Add(region);
                            hasChanges = true;
                        }
                    }
                }
            }
        }

        // Save changes if any new relationships were established
        if (hasChanges)
            await dbContext.SaveChangesAsync(cancellationToken);
    }
}