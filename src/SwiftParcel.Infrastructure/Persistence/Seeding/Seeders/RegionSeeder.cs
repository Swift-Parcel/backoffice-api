using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class RegionSeeder : IEntitySeeder
{
    public int Order => 40;

    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Regions.AnyAsync(cancellationToken))
            return;

        var countriesByCode = await dbContext.Countries
            .ToDictionaryAsync(c => c.CountryCode, c => c, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var legacyRegions = await dbContext.Database
            .SqlQueryRaw<LegacyRegionDto>(@"
                SELECT 
                    id, region_name, country_code, country_name, timezone, 
                    business_hours_start, business_hours_end, business_days, 
                    manager_email, is_active 
                FROM regions")
            .ToListAsync(cancellationToken);

        var newRegions = new List<Region>();

        foreach (var oldRegion in legacyRegions)
        {
            bool isActive = oldRegion.is_active?.Trim().ToLowerInvariant() is "yes" or "true" or "1";
            // TODO: Do we need non-active Regions? if no, delete comment
            // if (!isActive)
            //     continue;

            // Parse Times
            var start = ParseTimeOnly(oldRegion.business_hours_start);
            var end = ParseTimeOnly(oldRegion.business_hours_end);

            // Parse Business Days
            var days = ParseBusinessDays(oldRegion.business_days);

            countriesByCode.TryGetValue(oldRegion.country_code ?? string.Empty, out var country);

            var newRegion = new Region
            {
                Id = StringParserHelper.ExtractIntegerId(oldRegion.id),
                RegionName = oldRegion.region_name ?? string.Empty,
                CountryCode = oldRegion.country_code ?? string.Empty,
                Country = country!,
                BusinessHoursStart = start,
                BusinessHoursEnd = end,
                BusinessDays = days,
                ManagerEmail = oldRegion.manager_email ?? string.Empty,
                IsActive = isActive
            };

            newRegions.Add(newRegion);
        }

        await dbContext.Regions.AddRangeAsync(newRegions, cancellationToken);
    }

    private static TimeOnly ParseTimeOnly(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new TimeOnly(8, 0);
        }

        var formats = new[] { "HH:mm", "H:mm", "h:mm tt", "hh:mm tt", "H:mm:ss" };
        if (TimeOnly.TryParseExact(input.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            return result;
        }

        return new TimeOnly(8, 0);
    }

    private static List<DayOfWeek> ParseBusinessDays(string? input)
    {
        var defaultDays = new List<DayOfWeek> 
        { 
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday 
        };

        if (string.IsNullOrWhiteSpace(input))
        {
            return defaultDays;
        }

        var val = input.Trim();

        if (val.Equals("Mon-Fri", StringComparison.OrdinalIgnoreCase) || 
            val.Equals("Monday-Friday", StringComparison.OrdinalIgnoreCase))
        {
            return defaultDays;
        }

        var results = new List<DayOfWeek>();
        var parts = StringParserHelper.ParseCsvString(val);

        foreach (var part in parts)
        {
            if (int.TryParse(part, out int dayNum) && dayNum >= 1 && dayNum <= 7)
            {
                var day = dayNum == 7 ? DayOfWeek.Sunday : (DayOfWeek)dayNum;
                results.Add(day);
                continue;
            }

            if (Enum.TryParse<DayOfWeek>(part, true, out var parsedDay))
            {
                results.Add(parsedDay);
            }
        }

        return results.Count > 0 ? results : defaultDays;
    }

    private record LegacyRegionDto(
        string id,
        string region_name,
        string country_code,
        string country_name,
        string timezone,
        string business_hours_start,
        string business_hours_end,
        string business_days,
        string manager_email,
        string is_active);
}