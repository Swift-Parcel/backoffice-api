using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class CountrySeeder : IEntitySeeder
{
    public int Order => 10;

    public Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Countries.AnyAsync(cancellationToken))
            return;

        var legacyRegions = await oldDbContext.Database
            .SqlQueryRaw<string>("SELECT DISTINCT region FROM handlers WHERE region IS NOT NULL AND region != ''")
            .ToListAsync(cancellationToken);

        var legacyAddresses = await dbContext.Database
            .SqlQueryRaw<string>("SELECT address FROM customers WHERE address IS NOT NULL AND address != ''")
            .Concat(dbContext.Database.SqlQueryRaw<string>("SELECT recipient_address FROM parcels WHERE recipient_address IS NOT NULL AND recipient_address != ''"))
            .ToListAsync(cancellationToken);

        var allLocationTexts = legacyRegions
            .Concat(legacyAddresses)
            .Distinct(StringComparer.OrdinalIgnoreCase);

        var detectedCountries = new Dictionary<string, (string Name, string Timezone)>(StringComparer.OrdinalIgnoreCase);

        foreach (var text in allLocationTexts)
        {
            if (ContainsAny(text, "Budapest", "Debrecen", "Szeged", "Hungary", "HU"))
            {
                detectedCountries["HU"] = ("Hungary", "Europe/Budapest");
            }
            if (ContainsAny(text, "Wien", "Vienna", "Graz", "Salzburg", "Linz", "Austria", "AT"))
            {
                detectedCountries["AT"] = ("Austria", "Europe/Vienna");
            }
            if (ContainsAny(text, "Praha", "Prague", "Brno", "Czech", "CZ"))
            {
                detectedCountries["CZ"] = ("Czech Republic", "Europe/Prague");
            }
            if (ContainsAny(text, "Warszawa", "Warsaw", "Kraków", "Gdańsk", "Poland", "PL"))
            {
                detectedCountries["PL"] = ("Poland", "Europe/Warsaw");
            }
        }

        var newCountries = detectedCountries.Select(c => new Country
        {
            CountryCode = c.Key,
            CountryName = c.Value.Name,
            TimeZone = c.Value.Timezone
        }).ToList();

        await dbContext.Countries.AddRangeAsync(newCountries, cancellationToken);
    }

    private static bool ContainsAny(string text, params string[] keywords)
    {
        return keywords.Any(kw => text.Contains(kw, StringComparison.OrdinalIgnoreCase));
    }
}