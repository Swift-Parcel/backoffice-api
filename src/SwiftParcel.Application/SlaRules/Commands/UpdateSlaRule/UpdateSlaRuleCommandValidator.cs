using FluentValidation;

namespace SwiftParcel.Application.SlaRules.Commands.UpdateSlaRule;

public class UpdateSlaRuleCommandValidator : AbstractValidator<UpdateSlaRuleCommand>
{
    public UpdateSlaRuleCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("SLA Rule ID must be greater than 0.");

        RuleFor(x => x.SlaHours)
            .GreaterThanOrEqualTo(0).WithMessage("SLA hours cannot be negative.");

        RuleFor(x => x.EscalationAfter)
            .GreaterThanOrEqualTo(0)
            .When(x => x.EscalationAfter.HasValue)
            .WithMessage("Escalation hours cannot be negative.");
    }
}