namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

using Microsoft.EntityFrameworkCore;
using Interfaces;
using Helpers;

public class StatusWorkflowSeeder : IEntitySeeder
{
    public int Order => 11;

    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        // Build a case-insensitive lookup dictionary mapping role names to Role entities
        var roleMap = await dbContext.Roles
            .ToDictionaryAsync(r => r.RoleName, r => r, StringComparer.OrdinalIgnoreCase, cancellationToken);

        // Fetch existing StatusWorkflows with their loaded AllowedRoles to prevent duplicate assignments
        var workflows = await dbContext.StatusWorkflows
            .Include(sw => sw.AllowedRoles)
            .ToDictionaryAsync(sw => sw.Id, cancellationToken);

        // Raw SQL query to fetch allowed roles from legacy status_workflow table
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT id, allowed_roles FROM status_workflow WHERE allowed_roles IS NOT NULL AND allowed_roles != ''";

        await dbContext.Database.OpenConnectionAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var hasChanges = false;

        // Process results row-by-row
        while (await reader.ReadAsync(cancellationToken))
        {
            var workflowId = reader.GetInt32(0);
            var rawRoles = reader.GetString(1);

            // Parse and clean CSV role names
            var roleNames = StringParserHelper.ParseCsvString(rawRoles);

            if (workflows.TryGetValue(workflowId, out var currentWorkflow))
            {
                foreach (var roleName in roleNames)
                {
                    // Match role name and ensure it is not already linked to the StatusWorkflow
                    if (roleMap.TryGetValue(roleName, out var role))
                    {
                        if (currentWorkflow.AllowedRoles.All(r => r.Id != role.Id))
                        {
                            currentWorkflow.AllowedRoles.Add(role);
                            hasChanges = true;
                        }
                    }
                }
            }
        }

        // Save changes if any new relationships were established
        if (hasChanges)
            await dbContext.SaveChangesAsync(cancellationToken);
    }
}