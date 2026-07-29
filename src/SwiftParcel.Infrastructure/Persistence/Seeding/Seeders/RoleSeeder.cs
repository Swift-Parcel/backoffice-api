using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class RoleSeeder : IEntitySeeder
{
    public int Order => 30;

    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Roles.AnyAsync(cancellationToken))
        {
            return;
        }

        // Cache for permissions lookup by Name or Code
        var permissionsByName = await dbContext.Permissions
            .ToDictionaryAsync(p => p.Name, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var legacyRoles = await oldDbContext.Database
            .SqlQueryRaw<LegacyRoleDto>(@"
                SELECT 
                    id, role_name, description, permissions, 
                    can_access_all_regions, is_active, created_date 
                FROM roles")
            .ToListAsync(cancellationToken);

        var newRoles = new List<Role>();

        foreach (var oldRole in legacyRoles)
        {
            // Parse Booleans
            bool canAccessAllRegions = oldRole.can_access_all_regions?.Trim().ToLowerInvariant() is "yes" or "true" or "1";
            bool isActive = oldRole.is_active?.Trim().ToLowerInvariant() is "yes" or "true" or "1";

            var newRole = new Role
            {
                Id = StringParserHelper.ExtractIntegerId(oldRole.id),
                RoleName = oldRole.role_name ?? string.Empty,
                Description = oldRole.description ?? string.Empty,
                CanAccessAllRegions = canAccessAllRegions,
                IsActive = isActive,
                CreatedDate = TimestampParserHelper.ParseOrFallback(oldRole.created_date)
            };

            // Process Permissions (Many-to-Many)
            if (!string.IsNullOrWhiteSpace(oldRole.permissions))
            {
                var permissionNames = StringParserHelper.ParseCsvString(oldRole.permissions);
                foreach (var permName in permissionNames)
                {
                    if (permName == "*")
                    {
                        // Wildcard: add all existing permissions
                        foreach (var perm in permissionsByName.Values)
                        {
                            newRole.Permissions.Add(perm);
                        }
                    }
                    else if (permissionsByName.TryGetValue(permName, out var perm))
                    {
                        newRole.Permissions.Add(perm);
                    }
                }
            }

            newRoles.Add(newRole);
        }

        await dbContext.Roles.AddRangeAsync(newRoles, cancellationToken);
    }

    private record LegacyRoleDto(
        string id,
        string role_name,
        string description,
        string permissions,
        string can_access_all_regions,
        string is_active,
        string created_date);
}