using SwiftParcel.Application.DTO;

namespace SwiftParcel.Application.Integration.Interfaces;

public interface ISlaRuleService
{
    Task<SlaRuleResponse?> UpdateSlaRuleAsync(int id, UpdateSlaRuleRequest request, CancellationToken cancellationToken = default);
}