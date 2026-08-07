using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Infrastructure.Persistence.Repositories;

public class SlaRuleRepository(AppDbContext dbContext) : ISlaRuleRepository
{
    public async Task<SlaRule?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.SlaRules
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task ReplaceRuleAsync(SlaRule oldRule, SlaRule newRule, CancellationToken cancellationToken = default)
    {
        dbContext.SlaRules.Update(oldRule);
        dbContext.SlaRules.Add(newRule);
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}