using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class SystemConfigSeeder : IEntitySeeder
{
    public int Order => 20;
    
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.SystemConfigs.AnyAsync(cancellationToken))
        {
            return;
        }

        var legacyConfigs = await dbContext.Database
            .SqlQueryRaw<LegacyConfigDto>("SELECT id, config_key, config_value, description, updated_by, updated_date FROM system_config")
            .ToListAsync(cancellationToken);

        var newConfigs = new List<SystemConfig>();

        var userLookup = await SeedingLookupHelper.GetUserLookupByUsernameAsync(dbContext, cancellationToken);
        
        foreach (var legacyConfig in legacyConfigs)
        {
            var newConfig = new SystemConfig
            {
                Id = StringParserHelper.ExtractIntegerId(legacyConfig.id),
                ConfigKey = legacyConfig.config_key,
                ConfigValue = StringParserHelper.ParseJsonDocument(legacyConfig.config_value),
                Description = legacyConfig.description,
                UpdatedById = userLookup[legacyConfig.updated_by],
                UpdatedDate = TimestampParserHelper.ParseOrFallback(legacyConfig.updated_date)
            };

            newConfigs.Add(newConfig);
        }

        await dbContext.SystemConfigs.AddRangeAsync(newConfigs, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken); 
    }

    private record LegacyConfigDto(
       string id,
       string config_key,
       string config_value,
       string description,
       string updated_by,
       string updated_date);
}