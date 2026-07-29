namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Helpers;

public class UserSeeder : BaseCsvRelationSeeder<User, Region>
{
    private List<Region> _allRegions = new();

    public override int Order => 5;

    protected override string SqlQuery =>
        "SELECT id, regions FROM users WHERE regions IS NOT NULL AND regions != ''";

    protected override async Task<Dictionary<int, User>> GetEntitiesAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        _allRegions = await dbContext.Regions.ToListAsync(cancellationToken);

        var users = await dbContext.Users
            .Include(u => u.Regions)
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        // Process roles according to RBAC (1 User = 1 Role)
        var roleMap = await dbContext.Roles
            .ToDictionaryAsync(r => r.RoleName, r => r.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT id, roles FROM users WHERE roles IS NOT NULL AND roles != ''";

        await dbContext.Database.OpenConnectionAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var userId = reader.GetInt32(0);
            var rawRoles = reader.GetString(1);

            var roleTokens = StringParserHelper.ParseCsvString(rawRoles);

            if (users.TryGetValue(userId, out var user))
            {
                foreach (var token in roleTokens)
                {
                    if (roleMap.TryGetValue(token, out var roleId))
                    {
                        user.RoleId = roleId;
                        break;
                    }
                }
            }
        }

        return users;
    }

    protected override Task<List<Region>> ResolveTargetsAsync(AppDbContext dbContext, string token, CancellationToken cancellationToken)
    {
        if (token.Equals("ALL", StringComparison.OrdinalIgnoreCase) || token == "*")
        {
            return Task.FromResult(_allRegions);
        }

        var found = _allRegions.Where(r =>
            string.Equals(r.CountryCode, token, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(r.Name, token, StringComparison.OrdinalIgnoreCase)).ToList();

        return Task.FromResult(found);
    }

    protected override bool RelationExists(User entity, Region target) => entity.Regions.Any(r => r.Id == target.Id);

    protected override void AttachRelation(User entity, Region target) => entity.Regions.Add(target);
}