namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Enums;
using Helpers;
using Interfaces;

public class StatusWorkflowSeeder : IEntitySeeder
{
    public int Order => 150;

    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.StatusWorkflows.AnyAsync(cancellationToken))
        {
            return;
        }

        // Cache for Roles lookup by RoleName
        var rolesByName = await dbContext.Roles
            .ToDictionaryAsync(r => r.RoleName, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var legacyWorkflows = await oldDbContext.Database
            .SqlQueryRaw<LegacyStatusWorkflowDto>(@"
                SELECT 
                    id, from_status, to_status, require_note, 
                    require_resolution, allowed_roles, is_active 
                FROM status_workflow")
            .ToListAsync(cancellationToken);

        var newWorkflows = new List<StatusWorkflow>();

        foreach (var oldWorkflow in legacyWorkflows)
        {
            // Parse Enums for FromStatus and ToStatus
            CaseStatus? fromStatus = null;
            if (!string.IsNullOrWhiteSpace(oldWorkflow.from_status) &&
                Enum.TryParse<CaseStatus>(oldWorkflow.from_status.Replace(" ", ""), true, out var parsedFrom))
            {
                fromStatus = parsedFrom;
            }

            CaseStatus? toStatus = null;
            if (!string.IsNullOrWhiteSpace(oldWorkflow.to_status) &&
                Enum.TryParse<CaseStatus>(oldWorkflow.to_status.Replace(" ", ""), true, out var parsedTo))
            {
                toStatus = parsedTo;
            }

            // Parse Booleans
            bool requireNote = oldWorkflow.require_note?.Trim().ToLowerInvariant() is "yes" or "true" or "1";
            bool requireResolution = oldWorkflow.require_resolution?.Trim().ToLowerInvariant() is "yes" or "true" or "1";
            bool isActive = oldWorkflow.is_active?.Trim().ToLowerInvariant() is "yes" or "true" or "1";

            var newWorkflow = new StatusWorkflow
            {
                Id = StringParserHelper.ExtractInteger(oldWorkflow.id),
                FromStatus = fromStatus,
                ToStatus = toStatus,
                RequireNote = requireNote,
                RequireResolution = requireResolution,
                IsActive = isActive
            };

            // Process AllowedRoles (Many-to-Many)
            if (!string.IsNullOrWhiteSpace(oldWorkflow.allowed_roles))
            {
                var roleNames = StringParserHelper.ParseCsvString(oldWorkflow.allowed_roles);
                foreach (var roleName in roleNames)
                {
                    if (rolesByName.TryGetValue(roleName, out var role))
                    {
                        newWorkflow.AllowedRoles.Add(role);
                    }
                }
            }

            newWorkflows.Add(newWorkflow);
        }

        await dbContext.StatusWorkflows.AddRangeAsync(newWorkflows, cancellationToken);
    }

    private record LegacyStatusWorkflowDto(
        string id,
        string from_status,
        string to_status,
        string require_note,
        string require_resolution,
        string allowed_roles,
        string is_active);
}