namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;

public class RoleSeeder : BaseCsvRelationSeeder<Role, Permission>
{
    private List<Permission> _allPermissions = new();

    public override int Order => 3;

    protected override string SqlQuery =>
        "SELECT id, permissions FROM roles WHERE permissions IS NOT NULL AND permissions != ''";

    protected override async Task<Dictionary<int, Role>> GetEntitiesAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        _allPermissions = await dbContext.Permissions.ToListAsync(cancellationToken);

        return await dbContext.Roles
            .Include(r => r.Permissions)
            .ToDictionaryAsync(r => r.Id, cancellationToken);
    }

    protected override Task<List<Permission>> ResolveTargetsAsync(AppDbContext dbContext, string token, CancellationToken cancellationToken)
    {
        // Wildcard '*' maps to all available permissions
        if (token == "*")
        {
            return Task.FromResult(_allPermissions);
        }

        var found = _allPermissions
            .Where(p => string.Equals(p.Name, token, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult(found);
    }

    protected override bool RelationExists(Role entity, Permission target) =>
        entity.Permissions.Any(p => p.Id == target.Id);

    protected override void AttachRelation(Role entity, Permission target) =>
        entity.Permissions.Add(target);
}