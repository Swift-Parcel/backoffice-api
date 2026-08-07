using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.SlaRules.Commands.UpdateSlaRule;

public class UpdateSlaRuleCommandHandler(ISlaRuleRepository slaRuleRepository) 
    : IRequestHandler<UpdateSlaRuleCommand, Result<SlaRuleResponse>>
{
    public async Task<Result<SlaRuleResponse>> Handle(UpdateSlaRuleCommand request, CancellationToken cancellationToken)
    {
        var oldRule = await slaRuleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (oldRule == null)
        {
            return Result<SlaRuleResponse>.Failure(
                Error.NotFound("slarule_not_found", $"SLA Rule with ID {request.Id} was not found."));
        }

        oldRule.IsActive = false;

        var newRule = new SlaRule
        {
            Name = oldRule.Name,
            CaseType = request.CaseType,
            Priority = request.Priority,
            ServiceType = request.ServiceType,
            SlaHours = request.SlaHours,
            IsBusinessHours = request.IsBusinessHours,
            EscalationAfter = request.EscalationAfter ?? 0,
            EscalationHandlerId = request.EscalationHandlerId,
            EscalationDepartment = request.EscalationDepartment,
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            Notes = request.Notes
        };

        await slaRuleRepository.ReplaceRuleAsync(oldRule, newRule, cancellationToken);

        var response = new SlaRuleResponse(
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

        return Result<SlaRuleResponse>.Success(response);
    }
}