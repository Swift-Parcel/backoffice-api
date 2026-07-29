using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class PermissionSeeder : IEntitySeeder
{
    public int Order => 20;

    public static List<string> granularPermissions = new List<string>
    {
        "case.delete",
        "case.export",
        "case.merge",
        "config.sla.edit",
        "config.template.edit",
        "customer.edit",
        "report.financial"
    };
    
    public static List<string> rolePermissions = new List<string>
    {
        "case.view",
        "case.create",
        "case.edit",
        "case.assign",
        "case.status.change",
        "case.escalate",
        "customer.view",
        "report.view",
        "handler.manage",
        "parcel.view",
        "note.view",
        "note.create"
    };
    
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Permissions.AnyAsync(cancellationToken))
        {
            return;
        }
        
        var newPermissions = new List<Permission>();
        
        foreach (var granularPermission in granularPermissions)
        {
            var newPermission = new Permission
            {
                Name = granularPermission,
                Granular = true
            };
            
            newPermissions.Add(newPermission);
        }

        foreach (var rolePermission in rolePermissions)
        {
            var newPermission = new Permission
            {
                Name = rolePermission,
                Granular = false
            };
            
            newPermissions.Add(newPermission);
        }
        
        await dbContext.Permissions.AddRangeAsync(newPermissions, cancellationToken);
    }
}