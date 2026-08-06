using FluentValidation;

namespace SwiftParcel.Application.Cases.Commands.AssignCase;

public class AssignCaseCommandValidator : AbstractValidator<AssignCaseCommand>
{
    public AssignCaseCommandValidator()
    {
        RuleFor(x => x.CaseNumber)
            .NotEmpty().WithMessage("Case Number is required.");

        RuleFor(x => x.HandlerId)
            .NotEmpty().WithMessage("A Handler ID is required.")
            .GreaterThan(0).WithMessage("A valid Handler ID is required.");
    }
}