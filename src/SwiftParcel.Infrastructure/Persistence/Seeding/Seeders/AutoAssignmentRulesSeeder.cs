using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class AutoAssignmentRuleSeeder : IEntitySeeder
{
    public int Order => 160;

    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.AutoAssignmentRules.AnyAsync(cancellationToken))
            return;

        // Cache existing active users by ID and FullName/Username for handler resolution
        var usersById = await dbContext.Users
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var usersByName = await dbContext.Users
            .ToDictionaryAsync(u => u.FullName, u => u.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var usersByUsername = await dbContext.Users
            .ToDictionaryAsync(u => u.Username, u => u.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var legacyRules = await oldDbContext.Database
            .SqlQueryRaw<LegacyAutoAssignmentRuleDto>(@"
                SELECT 
                    id, rule_name, priority, conditions, 
                    assign_to_handler_id, assign_to_handler_name, 
                    assign_to_department, is_active, created_date, notes 
                FROM auto_assignment_rules")
            .ToListAsync(cancellationToken);

        var newRules = new List<AutoAssignmentRule>();

        foreach (var oldRule in legacyRules)
        {
            // Parse IsActive boolean
            bool isActive = oldRule.is_active?.Trim().ToLowerInvariant() is "yes" or "true" or "1";

            // Parse Priority integer
            int priority = 0;
            if (!string.IsNullOrWhiteSpace(oldRule.priority))
                int.TryParse(oldRule.priority.Trim(), out priority);

            // Resolve AssignToHandler (UserId)
            int handlerId = 0;

            // 1. Try by ID
            if (!string.IsNullOrWhiteSpace(oldRule.assign_to_handler_id))
            {
                int parsedId = StringParserHelper.ExtractInteger(oldRule.assign_to_handler_id);
                if (usersById.ContainsKey(parsedId))
                    handlerId = parsedId;
            }

            // 2. Fallback by FullName or Username if ID was missing/invalid
            if (handlerId == 0 && !string.IsNullOrWhiteSpace(oldRule.assign_to_handler_name))
            {
                var handlerName = oldRule.assign_to_handler_name.Trim();
                if (usersByName.TryGetValue(handlerName, out var uId))
                    handlerId = uId;

                else if (usersByUsername.TryGetValue(handlerName, out var uUsernameId))
                    handlerId = uUsernameId;
            }

            var newRule = new AutoAssignmentRule
            {
                Id = StringParserHelper.ExtractInteger(oldRule.id),
                RuleName = oldRule.rule_name ?? string.Empty,
                Priority = priority,
                Conditions = oldRule.conditions ?? string.Empty,
                AssignToHandler = handlerId,
                AssignToDepartment = oldRule.assign_to_department ?? string.Empty,
                IsActive = isActive,
                CreatedDate = TimestampParserHelper.ParseOrFallback(oldRule.created_date),
                Notes = oldRule.notes ?? string.Empty
            };

            newRules.Add(newRule);
        }

        await dbContext.AutoAssignmentRules.AddRangeAsync(newRules, cancellationToken);
    }

    private record LegacyAutoAssignmentRuleDto(
        string id,
        string rule_name,
        string priority,
        string conditions,
        string assign_to_handler_id,
        string assign_to_handler_name,
        string assign_to_department,
        string is_active,
        string created_date,
        string notes);
}