namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;

public class StatusWorkflowSeeder : BaseCsvRelationSeeder<StatusWorkflow, Role>
{
    private Dictionary<string, Role> _roleMap = new();

    public override int Order => 11;
    protected override string SqlQuery => "SELECT id, allowed_roles FROM status_workflow WHERE allowed_roles IS NOT NULL AND allowed_roles != ''";

    protected override async Task<Dictionary<int, StatusWorkflow>> GetEntitiesAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        _roleMap = await dbContext.Roles.ToDictionaryAsync(r => r.RoleName, r => r, StringComparer.OrdinalIgnoreCase, cancellationToken);
        return await dbContext.StatusWorkflows.Include(sw => sw.AllowedRoles).ToDictionaryAsync(sw => sw.Id, cancellationToken);
    }

    protected override Task<List<Role>> ResolveTargetsAsync(AppDbContext dbContext, string token, CancellationToken cancellationToken)
    {
        var result = _roleMap.TryGetValue(token, out var role) ? new List<Role> { role } : new List<Role>();
        return Task.FromResult(result);
    }

    protected override bool RelationExists(StatusWorkflow entity, Role target) => entity.AllowedRoles.Any(r => r.Id == target.Id);
    protected override void AttachRelation(StatusWorkflow entity, Role target) => entity.AllowedRoles.Add(target);
}