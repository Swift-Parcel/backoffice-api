namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

using Microsoft.EntityFrameworkCore;
using Interfaces;
using Domain.Entities;
using Helpers;

public class StatusWorkflowSeeder : IEntitySeeder
{
    public int Order => 11;

    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        // Build a case-insensitive lookup dictionary for roles (O(1) lookup time)
        var roleMap = await dbContext.Roles
            .ToDictionaryAsync(r => r.RoleName, r => r.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        // Load existing relations into a HashSet to prevent duplicate insertions
        var existingWorkflowRoles = await dbContext.StatusWorkflowRoles
            .Select(swr => new { swr.WorkflowId, swr.RoleId })
            .ToHashSetAsync(cancellationToken);

        var workflowRolesToInsert = new List<StatusWorkflowRole>();

        // Raw SQL query to fetch allowed roles from legacy status_workflow table
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT id, allowed_roles FROM status_workflow WHERE allowed_roles IS NOT NULL AND allowed_roles != ''";

        await dbContext.Database.OpenConnectionAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        // Process results row-by-row
        while (await reader.ReadAsync(cancellationToken))
        {
            var workflowId = reader.GetInt32(0);
            var rawRoles = reader.GetString(1);

            // Parse and clean CSV role names
            var roleNames = StringParserHelper.ParseCsvString(rawRoles);

            foreach (var roleName in roleNames)
            {
                // Match role name against the new Roles table ID
                if (roleMap.TryGetValue(roleName, out var roleId))
                {
                    var pair = new { WorkflowId = workflowId, RoleId = roleId };

                    // Ensure record doesn't exist in the database or in the current batch
                    if (!existingWorkflowRoles.Contains(pair) && !workflowRolesToInsert.Any(swr => swr.WorkflowId == workflowId && swr.RoleId == roleId))
                    {
                        workflowRolesToInsert.Add(new StatusWorkflowRole
                        {
                            WorkflowId = workflowId,
                            RoleId = roleId
                        });
                    }
                }
            }
        }

        // Bulk insert mapped entities in a single transaction
        if (workflowRolesToInsert.Any())
        {
            await dbContext.StatusWorkflowRoles.AddRangeAsync(workflowRolesToInsert, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}