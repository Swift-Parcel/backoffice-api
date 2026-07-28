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
        HashSet<string> otherTags = new()
        {
            "vip",
            "multiple_parcels"
        };

        return StringParserHelpers.GetEnumNamesSnakeCase<Tag>()
            .Union(StringParserHelpers.GetEnumNamesLowercase<Tag>())
            .Union(otherTags)
            .ToHashSet();
    }


    private static IEnumerable<string> CleanTagList(IEnumerable<string> tags)
    {
        var cleanedTags = new List<string>();
        foreach (var tag in tags)
        {
            if (!BannedTags.Contains(tag))
            {
                cleanedTags.Add(tag);
            }
        }
        return cleanedTags;
    }
    
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Tags.AnyAsync(cancellationToken))
        {
            return;
        }
        
        var legacyConfigs = await dbContext.Database
            .SqlQueryRaw<LegacyTagDto>("SELECT tags FROM cases")
            .ToListAsync(cancellationToken);
        
        var uniqueTagNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        foreach (var legacyConfig in legacyConfigs)
        {
            var legacyTags = StringParserHelper.ParseCsvString(legacyConfig.tags);
            var cleanTags = CleanTagList(legacyTags);
            
            foreach (var cleanTag in cleanTags)
            {
                uniqueTagNames.Add(cleanTag);
            }
        }
        var newTags = uniqueTagNames.Select(tagName => new Tag { Name = tagName }).ToList();
        
        await dbContext.Tags.AddRangeAsync(newTags, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    private record LegacyTagDto(
        string tags);
}