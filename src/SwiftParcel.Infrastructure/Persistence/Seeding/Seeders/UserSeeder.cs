namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Helpers;
using Interfaces;


public class UserSeeder : IEntitySeeder
{
    public int Order => 50;

    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Users.AnyAsync(cancellationToken))
            return;

        var rolesById = await dbContext.Roles
            .ToDictionaryAsync(r => r.Id, cancellationToken);

        var regionsByName = await dbContext.Regions
            .ToDictionaryAsync(r => r.Name, r => r, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var legacyUsers = await oldDbContext.Database
            .SqlQueryRaw<LegacyUserDto>(@"
                SELECT 
                    id, username, password, full_name, email, 
                    role, role_id, regions, is_active, 
                    last_login, created_date, created_by 
                FROM users")
            .ToListAsync(cancellationToken);

        var newUsers = new List<User>();
        // Temporary tracking for 2-pass CreatedBy resolving: UserId -> CreatedByUsername string
        var createdByMap = new Dictionary<int, string>();

        foreach (var oldUser in legacyUsers)
        {
            int userId = StringParserHelper.ExtractIntegerId(oldUser.id);

            // 1. Resolve Primary RoleId
            int roleId = 1;
            if (!string.IsNullOrWhiteSpace(oldUser.role_id))
            {
                var roleIdStrings = StringParserHelper.ParseCsvString(oldUser.role_id);
                foreach (var rIdStr in roleIdStrings)
                {
                    int parsedId = StringParserHelper.ExtractIntegerId(rIdStr);
                    if (rolesById.ContainsKey(parsedId))
                    {
                        roleId = parsedId;
                        break;
                    }
                }
            }

            var newUser = new User
            {
                Id = userId,
                Username = oldUser.username ?? string.Empty,
                //TODO: Password Hash
                PasswordHash = oldUser.password ?? string.Empty,
                FullName = oldUser.full_name ?? string.Empty,
                Email = oldUser.email ?? string.Empty,
                RoleId = roleId,
                LastLogin = TimestampParserHelper.ParseOrFallback(oldUser.last_login),
                CreatedDate = TimestampParserHelper.ParseOrFallback(oldUser.created_date)
            };

            // 2. Resolve Regions (Many-to-Many)
            if (!string.IsNullOrWhiteSpace(oldUser.regions))
            {
                var regionNames = StringParserHelper.ParseCsvString(oldUser.regions);
                foreach (var regName in regionNames)
                {
                    if (regionsByName.TryGetValue(regName, out var region))
                    {
                        newUser.Regions.Add(region);
                    }
                }
            }

            // Store created_by string for the second pass
            if (!string.IsNullOrWhiteSpace(oldUser.created_by))
            {
                createdByMap[userId] = oldUser.created_by.Trim();
            }

            newUsers.Add(newUser);
        }

        // Pass 2: Resolve CreatedById using the created username map
        var usersByUsername = newUsers.ToDictionary(u => u.Username, u => u.Id, StringComparer.OrdinalIgnoreCase);

        foreach (var user in newUsers)
        {
            if (createdByMap.TryGetValue(user.Id, out var creatorUsername) &&
                usersByUsername.TryGetValue(creatorUsername, out var creatorId))
            {
                user.CreatedById = creatorId;
            }
            else
            {
                // Fallback: If creator is 'system' or not found, point to self
                user.CreatedById = user.Id;
            }
        }

        await dbContext.Users.AddRangeAsync(newUsers, cancellationToken);
    }

    private record LegacyUserDto(
        string id,
        string username,
        string password,
        string full_name,
        string email,
        string role,
        string role_id,
        string regions,
        string is_active,
        string last_login,
        string created_date,
        string created_by);
}