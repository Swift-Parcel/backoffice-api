using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public partial class SlaRuleSeeder : IEntitySeeder
{
    public int Order => 130;

    [GeneratedRegex(@"\s*[\-\–]\s*(updated|disabled|deprecated|old|v\d+).*", RegexOptions.IgnoreCase)]
    private static partial Regex RuleNameCleanerRegex();

    private static string CleanRuleName(string? ruleName)
    {
        if (string.IsNullOrWhiteSpace(ruleName))
        {
            return string.Empty;
        }

        return RuleNameCleanerRegex().Replace(ruleName, string.Empty).Trim();
    }
    
    private static HashSet<string> GetActiveRuleIds(IEnumerable<LegacySlaRuleDto> legacyRules)
    {
        return legacyRules
            .Select(dto => new
            {
                RawId = dto.id,
                CleanName = CleanRuleName(dto.rule_name),
                IsOriginallyActive = StringParserHelper.ParseBoolean(dto.is_active),
                CreatedDate = DateTime.TryParse(dto.created_date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                    ? date
                    : DateTime.MinValue
            })
            .GroupBy(x => x.CleanName)
            .Select(group => group
                .OrderByDescending(x => x.CreatedDate)
                .ThenByDescending(x => StringParserHelper.ExtractInteger(x.RawId))
                .FirstOrDefault()?.RawId)
            .Where(rawId => rawId != null)
            .ToHashSet()!;
    }
    
    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.SlaRules.AnyAsync(cancellationToken))
        {
            return;
        }
        
        var legacySlaRules = await oldDbContext.Database
            .SqlQueryRaw<LegacySlaRuleDto>("SELECT * FROM sla_rules")
            .ToListAsync(cancellationToken);
        
        var activeRuleIds = GetActiveRuleIds(legacySlaRules);
        
        var newSlaRules = new List<SlaRule>();
        
        var handlerLookup = await SeedingLookupHelper.GetHandlerLookupByNameAsync(dbContext, cancellationToken);
        
        foreach (var legacySlaRule in legacySlaRules)
        {
            var validCaseType = false;
            if (Enum.TryParse<CaseType>(legacySlaRule.case_type, ignoreCase: true, out var newCaseType))
            {
                validCaseType = true;
            }
            
            var validPriority = false;
            if (Enum.TryParse<Priority>(legacySlaRule.priority, ignoreCase: true, out var newPriority))
            {
                validPriority = true;
            }
            
            var validServiceType = false;
            if (Enum.TryParse<ServiceType>(legacySlaRule.service_type, ignoreCase: true, out var newServiceType))
            {
                validServiceType = true;
            }

            var hasHandlerId = false;
            if (handlerLookup.TryGetValue(legacySlaRule.escalation_target, out var newEscalationHandlerId))
            {
                hasHandlerId = true;
            }
            
            var newRule = new SlaRule
            {
                Id = StringParserHelper.ExtractInteger(legacySlaRule.id),
                Name = CleanRuleName(legacySlaRule.rule_name),
                CaseType = validCaseType ? newCaseType : null,
                Priority = validPriority ? newPriority : null,
                ServiceType = validServiceType ? newServiceType : null,
                SlaHours = StringParserHelper.ExtractInteger(legacySlaRule.sla_hours),
                IsBusinessHours = StringParserHelper.ParseBoolean(legacySlaRule.is_business_hours),
                EscalationAfter = StringParserHelper.ExtractInteger(legacySlaRule.escalation_after),
                EscalationHandlerId = hasHandlerId ? newEscalationHandlerId : null,
                EscalationDepartment = hasHandlerId ? null : legacySlaRule.escalation_target,
                IsActive = activeRuleIds.Contains(legacySlaRule.id),
                CreatedDate = TimestampParserHelper.ParseOrFallback(legacySlaRule.created_date),
                Notes = legacySlaRule.notes
            };
            
            newSlaRules.Add(newRule);
        }
        await dbContext.SlaRules.AddRangeAsync(newSlaRules, cancellationToken);
    }
    
    private record LegacySlaRuleDto(
        string id,
        string rule_name,
        string case_type,
        string priority,
        string service_type,
        string sla_hours,
        string is_business_hours,
        string escalation_after,
        string escalation_target,
        string is_active,
        string created_date,
        string notes);
}