using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Helpers;
using Interfaces;


public class EmailTemplateSeeder : IEntitySeeder
{
    public int Order => 170;

    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.EmailTemplates.AnyAsync(cancellationToken))
        {
            return;
        }

        // Cache for region lookups by name (e.g., "Vienna")
        var regionsByName = await dbContext.Regions
            .ToDictionaryAsync(r => r.Name, r => r.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        // Cache for user lookups by username (e.g., "admin")
        var usersByUsername = await dbContext.Users
            .ToDictionaryAsync(u => u.Username, u => u.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        int defaultAdminId = usersByUsername.TryGetValue("admin", out var adminId) 
            ? adminId 
            : 1;

        var legacyTemplates = await oldDbContext.Database
            .SqlQueryRaw<LegacyEmailTemplateDto>(@"
                SELECT 
                    id, template_name, language, region, 
                    subject, body, is_active, created_by, created_date 
                FROM email_templates")
            .ToListAsync(cancellationToken);

        var newTemplates = new List<EmailTemplate>();

        foreach (var oldTemplate in legacyTemplates)
        {
            // Parse IsActive
            bool isActive = oldTemplate.is_active?.Trim().ToLowerInvariant() is "yes" or "true" or "1";

            // TODO: IsActive in Emails?
            // if (!isActive) continue;

            // Resolve RegionId (if null/empty -> 0)
            int regionId = 0;
            if (!string.IsNullOrWhiteSpace(oldTemplate.region) &&
                regionsByName.TryGetValue(oldTemplate.region.Trim(), out var parsedRegionId))
            {
                regionId = parsedRegionId;
            }

            // Resolve CreatedBy (UserId)
            int createdById = defaultAdminId;
            if (!string.IsNullOrWhiteSpace(oldTemplate.created_by) &&
                usersByUsername.TryGetValue(oldTemplate.created_by.Trim(), out var uId))
            {
                createdById = uId;
            }

            var newTemplate = new EmailTemplate
            {
                Id = StringParserHelper.ExtractIntegerId(oldTemplate.id),
                TemplateName = oldTemplate.template_name ?? string.Empty,
                Language = oldTemplate.language ?? string.Empty,
                RegionId = regionId,
                Subject = oldTemplate.subject ?? string.Empty,
                Body = oldTemplate.body ?? string.Empty,
                IsActive = isActive,
                CreatedBy = createdById,
                CreatedDate = TimestampParserHelper.ParseOrFallback(oldTemplate.created_date)
            };

            newTemplates.Add(newTemplate);
        }

        await dbContext.EmailTemplates.AddRangeAsync(newTemplates, cancellationToken);
    }

    private record LegacyEmailTemplateDto(
        string id,
        string template_name,
        string language,
        string region,
        string subject,
        string body,
        string is_active,
        string created_by,
        string created_date);
}