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
            if (oldRole.id == "ROLE06")
                continue;
            
            var normalizedName = oldRole.role_name.Replace("-", "");
            
            var newRole = new Role
            {
                Id = StringParserHelper.ExtractInteger(oldRole.id),
                Name = normalizedName,
                Description = oldRole.description ?? string.Empty,
                CanAccessAllRegions = StringParserHelper.ParseBoolean(oldRole.can_access_all_regions),
                IsActive = StringParserHelper.ParseBoolean(oldRole.is_active),
                CreatedDate = TimestampParserHelper.ParseOrFallback(oldRole.created_date)
            };

            if (!string.IsNullOrWhiteSpace(oldRole.permissions))
            {
                var permissionNames = StringParserHelper.ParseCsvString(oldRole.permissions);
                foreach (var permName in permissionNames)
                {
                    if (permName == "*")
                    {
                        // Wildcard: add all existing permissions
                        foreach (var p in permissionsByName.Values)
                        {
                            newRole.Permissions.Add(p);
                        }
                    }
                    if (permissionsByName.TryGetValue(permName, out var perm))
                    {
                        newRole.Permissions.Add(perm);
                    }
                }
            }

            newRoles.Add(newRole);
        }

        await dbContext.Roles.AddRangeAsync(newRoles, cancellationToken);
    }

    private sealed record LegacyRoleDto(
        string id,
        string role_name,
        string description,
        string permissions,
        string can_access_all_regions,
        string is_active,
        string created_date);
}