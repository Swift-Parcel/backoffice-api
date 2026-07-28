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

        foreach (var oldConfig in legacyConfigs)
        {
            if (!TimestampParserHelper.TryParse(oldConfig.updated_date, out var updatedDate))
            {
                updatedDate = DateTime.UtcNow;
            }
            else
            {
                updatedDate = DateTime.SpecifyKind(updatedDate, DateTimeKind.Utc);
            }
            
            var newConfig = new SystemConfig
            {
                Id = StringParserHelper.ExtractIntegerId(oldConfig.id),
                ConfigKey = oldConfig.config_key,
                ConfigValue = StringParserHelper.ParseJsonDocument(oldConfig.config_value),
                Description = oldConfig.description,
                // UpdatedBy: match users.username to users.id
                UpdatedDate = updatedDate
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