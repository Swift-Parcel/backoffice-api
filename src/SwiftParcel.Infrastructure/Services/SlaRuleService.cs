using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.DTO;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence;

namespace SwiftParcel.Infrastructure.Services;

public class SlaRuleService : ISlaRuleService
{
    private readonly AppDbContext _dbContext;

    public SlaRuleService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SlaRuleResponse?> UpdateSlaRuleAsync(int id, UpdateSlaRuleRequest request, CancellationToken cancellationToken = default)
    {
        var oldRule = await _dbContext.SlaRules
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (oldRule == null)
            return null;

        oldRule.IsActive = false;

        var newRule = new SlaRule
        {
            Name = oldRule.Name,
            CaseType = request.CaseType,
            Priority = request.Priority,
            ServiceType = request.ServiceType,
            SlaHours = request.SlaHours,
            IsBusinessHours = request.IsBusinessHours,
            EscalationAfter = request.EscalationAfter,
            EscalationHandlerId = request.EscalationHandlerId,
            EscalationDepartment = request.EscalationDepartment,
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            Notes = request.Notes
        };

        _dbContext.SlaRules.Add(newRule);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SlaRuleResponse(
            newRule.Id,
            newRule.Name,
            newRule.CaseType,
            newRule.Priority,
            newRule.ServiceType,
            newRule.SlaHours,
            newRule.IsBusinessHours,
            newRule.EscalationAfter,
            newRule.EscalationHandlerId,
            newRule.EscalationDepartment,
            newRule.IsActive,
            newRule.CreatedDate,
            newRule.Notes
        );
    }
}