using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Common.Interfaces.Repositories;

public interface ISlaRuleRepository
{
    Task<SlaRule?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task ReplaceRuleAsync(SlaRule oldRule, SlaRule newRule, CancellationToken cancellationToken = default);
}