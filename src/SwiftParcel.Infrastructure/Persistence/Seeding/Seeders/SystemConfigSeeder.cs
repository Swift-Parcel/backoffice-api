using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class SystemConfigSeeder : IEntitySeeder
{
    public int Order => 170;

    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        if (await dbContext.SystemConfigs.AnyAsync(cancellationToken))
        {
            return;
        }

        var legacyConfigs = await oldDbContext.Database
            .SqlQueryRaw<LegacyConfigDto>(
                "SELECT id, config_key, config_value, description, updated_by, updated_date FROM system_config")
            .ToListAsync(cancellationToken);

        var newConfigs = new List<SystemConfig>();

        var userLookup = await SeedingLookupHelper.GetUserLookupByUsernameAsync(dbContext, cancellationToken);

        var uniqueLegacyConfigs = legacyConfigs
            .Where(c => !string.IsNullOrWhiteSpace(c.config_key))
            .GroupBy(c => c.config_key.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        foreach (var legacyConfig in uniqueLegacyConfigs)
        {
            int? updatedById = null;
            if (!string.IsNullOrWhiteSpace(legacyConfig.updated_by) &&
                userLookup.TryGetValue(legacyConfig.updated_by.Trim(), out var parsedUserId))
            {
                updatedById = parsedUserId;
            }

            var newConfig = new SystemConfig
            {
                Id = StringParserHelper.ExtractInteger(legacyConfig.id),
                ConfigKey = legacyConfig.config_key,
                ConfigValue = JsonParserHelper.ParseJsonDocument(legacyConfig.config_value),
                Description = legacyConfig.description,
                UpdatedById = updatedById ?? 1,
                UpdatedDate = TimestampParserHelper.ParseOrFallback(legacyConfig.updated_date)
            };

            newConfigs.Add(newConfig);
        }

        await dbContext.SystemConfigs.AddRangeAsync(newConfigs, cancellationToken);
    }

    private record LegacyConfigDto(
        string id,
        string config_key,
        string config_value,
        string description,
        string updated_by,
        string updated_date);
}