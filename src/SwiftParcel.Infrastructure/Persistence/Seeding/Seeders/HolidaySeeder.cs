using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class HolidaySeeder : IEntitySeeder
{
    public int Order => 110; 

    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Holidays.AnyAsync(cancellationToken))
        {
            return;
        }

        // Cache for regions (CountryCode or RegionName lookup)
        var allRegions = await dbContext.Regions.ToListAsync(cancellationToken);
        var regionsByCode = allRegions
            .ToDictionary(r => r.CountryCode, r => r, StringComparer.OrdinalIgnoreCase);

        var legacyHolidays = await dbContext.Database
            .SqlQueryRaw<LegacyHolidayDto>(@"
                SELECT 
                    id, holiday_name, holiday_date, region, is_recurring, notes 
                FROM holidays")
            .ToListAsync(cancellationToken);

        var newHolidays = new List<Holiday>();

        foreach (var oldHoliday in legacyHolidays)
        {
            // Parse Dates
            var (startDate, endDate) = ParseHolidayDates(oldHoliday.holiday_date);

            // Parse Boolean
            bool isRecurring = oldHoliday.is_recurring?.Trim().ToLowerInvariant() is "yes" or "true" or "1";

            var newHoliday = new Holiday
            {
                Id = StringParserHelper.ExtractIntegerId(oldHoliday.id),
                HolidayName = oldHoliday.holiday_name ?? string.Empty,
                StartDate = startDate,
                EndDate = endDate,
                IsRecurring = isRecurring,
                Notes = oldHoliday.notes ?? string.Empty
            };

            // Process Regions (Many-to-Many)
            if (!string.IsNullOrWhiteSpace(oldHoliday.region))
            {
                var regionCodes = StringParserHelper.ParseCsvString(oldHoliday.region);

                // If 'ALL', assign every available region
                if (regionCodes.Any(code => code.Equals("ALL", StringComparison.OrdinalIgnoreCase)))
                {
                    foreach (var region in allRegions)
                    {
                        newHoliday.Regions.Add(region);
                    }
                }
                else
                {
                    foreach (var code in regionCodes)
                    {
                        if (regionsByCode.TryGetValue(code, out var region))
                        {
                            newHoliday.Regions.Add(region);
                        }
                    }
                }
            }

            newHolidays.Add(newHoliday);
        }

        await dbContext.Holidays.AddRangeAsync(newHolidays, cancellationToken);
    }

    private static (DateTime StartDate, DateTime EndDate) ParseHolidayDates(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            var fallback = DateTime.UtcNow.Date;
            return (fallback, fallback);
        }

        var input = rawInput.Trim();
        int currentYear = DateTime.UtcNow.Year;

        // 1. If it's a range like "12-25 - 12-26"
        if (input.Contains('-') && input.Split('-').Length == 4) 
        {
            var parts = input.Split(new[] { " - ", "-" }, StringSplitOptions.TrimEntries);
            if (parts.Length == 4 && 
                int.TryParse(parts[0], out var startMonth) && int.TryParse(parts[1], out var startDay) &&
                int.TryParse(parts[2], out var endMonth) && int.TryParse(parts[3], out var endDay))
            {
                return (new DateTime(currentYear, startMonth, startDay), new DateTime(currentYear, endMonth, endDay));
            }
        }

        // 2. Try standard full date via TimestampParserHelper
        if (TimestampParserHelper.TryParse(input, out var fullDate))
        {
            return (fullDate, fullDate);
        }

        // 3. Month-Day formats (e.g. "01-01", "12-25")
        var monthDayParts = input.Split('-');
        if (monthDayParts.Length == 2 && 
            int.TryParse(monthDayParts[0], out var m) && 
            int.TryParse(monthDayParts[1], out var d))
        {
            var date = new DateTime(currentYear, m, d);
            return (date, date);
        }

        // 4. Text formats (e.g. "January 1")
        if (DateTime.TryParse($"{input} {currentYear}", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var textDate))
        {
            return (textDate, textDate);
        }

        var fallbackDate = DateTime.UtcNow.Date;
        return (fallbackDate, fallbackDate);
    }

    private record LegacyHolidayDto(
        string id,
        string holiday_name,
        string holiday_date,
        string region,
        string is_recurring,
        string notes);
}