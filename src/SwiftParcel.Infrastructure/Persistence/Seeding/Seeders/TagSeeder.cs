using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class TagSeeder : IEntitySeeder
{
    public int Order => 19;
    
    private static readonly HashSet<string> BannedTags = LoadBannedTags();
    private static HashSet<string> LoadBannedTags()
    {
        HashSet<string> OtherTags = new()
        {
            "vip",
            "multiple_parcels"
        };

        return StringParserHelpers.GetEnumNamesSnakeCase<Tag>()
            .Union(StringParserHelpers.GetEnumNamesLowercase<Tag>())
            .Union(OtherTags)
            .ToHashSet();
    }


    private static IEnumerable<string> CleanTagList(IEnumerable<string> tags)
    {
        foreach (var tag in tags)
        {
            
        }
    }
    
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Tags.AnyAsync(cancellationToken))
        {
            return;
        }
        
        var legacyConfigs = await dbContext.Database
            .SqlQueryRaw<LegacyConfigDto>("SELECT tags FROM cases")
            .ToListAsync(cancellationToken);
        
        var newConfigs = new List<LegacyConfigDto>();
        
        foreach (var legacyConfig in legacyConfigs)
        {
            var legacyTags = StringParserHelper.ParseCsvString(legacyConfig.tags);
            var cleanTags = CleanTagList(legacyTags);
            foreach (var cleanTag in cleanTags)
            {
                var newConfig = new Tag
                {
                    // Id: give incremental id automatically? idk how
                    Name = cleanTag
                };
                newConfigs.Add(newConfig);
            }
        }
        
        await dbContext.Tags.AddRangeAsync(newConfigs, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    private record LegacyConfigDto(
        string tags);
}