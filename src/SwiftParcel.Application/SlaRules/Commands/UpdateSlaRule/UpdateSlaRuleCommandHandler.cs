using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.SlaRules.Commands.UpdateSlaRule;

public class UpdateSlaRuleCommandHandler : IRequestHandler<UpdateSlaRuleCommand, Result<SlaRuleResponse>>
{
    private readonly IAppDbContext _context;

    public UpdateSlaRuleCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SlaRuleResponse>> Handle(UpdateSlaRuleCommand request, CancellationToken cancellationToken)
    {
        var oldRule = await _context.SlaRules
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

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

        _context.SlaRules.Add(newRule);

        await _context.SaveChangesAsync(cancellationToken);

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