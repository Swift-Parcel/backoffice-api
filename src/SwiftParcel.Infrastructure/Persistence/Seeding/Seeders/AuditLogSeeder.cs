using System.Net;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class AuditLogSeeder : IEntitySeeder
{
    public int Order => 190;

    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.AuditLogs.AnyAsync(cancellationToken))
            return;

        var usersList = await dbContext.Users.ToListAsync(cancellationToken);
        
        // Cache existing users for fast ID lookups
        var usersById = usersList.ToDictionary(u => u.Id);

        var usersByUsername = usersList
            .ToDictionary(u => u.Username, u => u.Id, StringComparer.OrdinalIgnoreCase);
        
        var usersByName = usersList
            .Where(u => !string.IsNullOrWhiteSpace(u.FullName))
            .GroupBy(u => u.FullName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);
        
        int defaultAdminUserId = usersByUsername.TryGetValue("admin", out var adminId) 
            ? adminId 
            : (usersById.Keys.FirstOrDefault());

        var legacyLogs = await oldDbContext.Database
            .SqlQueryRaw<LegacyAuditLogDto>(@"
                SELECT 
                    id, action, entity_type, entity_id, 
                    user_name, user_id, old_value, new_value, 
                    timestamp, ip_address, details 
                FROM audit_log")
            .ToListAsync(cancellationToken);
        
        var validLogs = legacyLogs.Where(l => l.user_id != "U99");

        var newLogs = new List<AuditLog>();

        foreach (var oldLog in validLogs)
        {
            // 1. Resolve UserId
            int userId = 0;

            if (!string.IsNullOrWhiteSpace(oldLog.user_id))
            {
                int parsedUserId = StringParserHelper.ExtractIntegerId(oldLog.user_id);
                if (usersById.ContainsKey(parsedUserId))
                {
                    userId = parsedUserId;
                }
            }

            // Fallback by username or full name if user_id wasn't matched
            if (userId == 0 && !string.IsNullOrWhiteSpace(oldLog.user_name))
            {
                var nameTrimmed = oldLog.user_name.Trim();
                if (usersByUsername.TryGetValue(nameTrimmed, out var uId))
                {
                    userId = uId;
                }
                else if (usersByName.TryGetValue(nameTrimmed, out var nId))
                {
                    userId = nId;
                }
            }

            // If still unresolved (e.g. "System", "U99" test user), fallback to default Admin user
            if (userId == 0)
            {
                userId = defaultAdminUserId;
            }

            // 2. Parse AuditAction Enum
            var action = ParseAuditAction(oldLog.action);

            // 3. Parse EntityType Enum
            var entityType = ParseEntityType(oldLog.entity_type);

            // 4. Parse IP Address
            IPAddress? ip = null;
            if (!string.IsNullOrWhiteSpace(oldLog.ip_address) && 
                IPAddress.TryParse(oldLog.ip_address.Trim(), out var parsedIp))
            {
                ip = parsedIp;
            }

            var newLog = new AuditLog
            {
                Id = StringParserHelper.ExtractIntegerId(oldLog.id),
                AuditAction = action,
                EntityType = entityType,
                EntityId = oldLog.entity_id,
                UserId = userId,
                OldValue = string.IsNullOrWhiteSpace(oldLog.old_value) ? null : oldLog.old_value,
                NewValue = string.IsNullOrWhiteSpace(oldLog.new_value) ? null : oldLog.new_value,
                TimeStamp = TimestampParserHelper.ParseOrFallback(oldLog.timestamp),
                IpAddress = ip,
                Details = string.IsNullOrWhiteSpace(oldLog.details) ? null : oldLog.details
            };

            newLogs.Add(newLog);
        }
        
        await dbContext.AuditLogs.AddRangeAsync(newLogs, cancellationToken);
    }

    private static AuditAction ParseAuditAction(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return default;
        }

        var normalized = input.Trim().ToUpperInvariant().Replace(".", "_").Replace(" ", "_");

        return normalized switch
        {
            "CREATE" or "CASE_CREATE" => AuditAction.Create,
            "UPDATE" or "CONFIG_UPDATE" => AuditAction.Update,
            "DELETE" => AuditAction.Delete,
            "STATUS_CHANGE" or "STATUS_UPDATE" => AuditAction.StatusChange,
            "ASSIGN" => AuditAction.Assign,
            "ESCALATE" => AuditAction.Escalate,
            "LOGIN" => AuditAction.LoginSucceeded,
            "LOGIN_FAILED" or "LOGIN_FAIL" => AuditAction.LoginFailed,
            "PERMISSION_GRANT" => AuditAction.PermissionGrant,
            "ROLE_CHANGE" => AuditAction.RoleChange,
            "NOTE_ADD" => AuditAction.NoteAdd,
            _ => Enum.TryParse<AuditAction>(normalized, true, out var result) ? result : default
        };
    }

    private static EntityType ParseEntityType(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return default;
        }

        var normalized = input.Trim().ToLowerInvariant().Replace("_", "").Replace(".", "");

        return normalized switch
        {
            "case" => EntityType.Case,
            "casenote" or "note" => EntityType.Note,
            "user" => EntityType.User,
            "userpermission" or "permission" => EntityType.UserPermission,
            "systemconfig" or "config" => EntityType.SystemConfig,
            _ => Enum.TryParse<EntityType>(input.Trim(), true, out var result) ? result : default
        };
    }

    private record LegacyAuditLogDto(
        string id,
        string action,
        string entity_type,
        string entity_id,
        string user_name,
        string user_id,
        string old_value,
        string new_value,
        string timestamp,
        string ip_address,
        string details);
}