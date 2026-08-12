using FluentValidation;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Commands.ChangeCaseStatusCommand;

public class ChangeCaseStatusValidator : AbstractValidator<ChangeCaseStatusCommand>
{
    public ChangeCaseStatusValidator()
    {
        RuleFor(x => x.CaseNumber)
            .NotEmpty()
            .WithMessage("Case number is required.");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid status provided.");

        RuleFor(x => x.Resolution)
            .MaximumLength(2000)
            .WithMessage("Resolution cannot exceed 2000 characters.");
            
        RuleFor(x => x.Resolution)
            .Empty()
            .When(x => x.NewStatus != CaseStatus.Resolved)
            .WithMessage("Resolution can only be provided when resolving a case.");
    }
}