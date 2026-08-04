using FluentValidation;

namespace SwiftParcel.Application.Cases.Commands.AssignCase;

public class AssignCaseCommandValidator : AbstractValidator<AssignCaseCommand>
{
    public AssignCaseCommandValidator()
    {
        RuleFor(x => x.CaseId)
            .GreaterThan(0).WithMessage("A valid Case ID is required.");

        RuleFor(x => x.HandlerId)
            .GreaterThan(0).WithMessage("A valid Handler ID is required.");
    }
}