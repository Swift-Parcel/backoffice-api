using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class UserPermissionSeeder : IEntitySeeder
{
    public int Order => 45;
    
    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.UserPermissions.AnyAsync(cancellationToken))
        {
            return;
        }
        
        var legacyUserPermissions = await oldDbContext.Database
            .SqlQueryRaw<LegacyUserPermissionDto>("SELECT * FROM user_permissions")
            .ToListAsync(cancellationToken);

        var userLookup = await SeedingLookupHelper.GetUserLookupByUsernameAsync(dbContext, cancellationToken);
        
        var permissionLookup = await dbContext.Permissions
            .ToDictionaryAsync(p => p.Name, p => p.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);
        
        var newUserPermissions = new List<UserPermission>();
        
        foreach (var legacyUserPermission in legacyUserPermissions)
        {
            // only granted permissions are stored
            if (legacyUserPermission.grant_type == "deny")
            {
                continue;
            }

            // User permissions can only be granular
            if (PermissionSeeder.rolePermissions.Contains(legacyUserPermission.permission))
            {
                continue;
            }
            
            DateTime? expires = TimestampParserHelper.TryParse(legacyUserPermission.expires, out var parsed) 
                ? parsed 
                : null;

            // if a permission expired, delete it
            if (expires != null && expires <= DateTime.UtcNow)
            {
                continue;
            }

            int userId;
            if (!string.IsNullOrWhiteSpace(legacyUserPermission.user_id) && 
                int.TryParse(legacyUserPermission.user_id, out var parsedUserId))
            {
                userId = parsedUserId;
            }
            else if (userLookup.TryGetValue(legacyUserPermission.user_id, out var idFromLookup))
            {
                userId = idFromLookup;
            }
            else
            {
                continue; 
            }
                
            if (!permissionLookup.TryGetValue(legacyUserPermission.permission, out var permissionId))
            {
                continue;
            }
                
            var newUserPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId,
                Expires = expires
            };
            
            newUserPermissions.Add(newUserPermission);
        }
       
        await dbContext.UserPermissions.AddRangeAsync(newUserPermissions, cancellationToken);
    }

    private record LegacyUserPermissionDto(
        string user_id,
        string permission,
        string grant_type,
        string? expires);
}