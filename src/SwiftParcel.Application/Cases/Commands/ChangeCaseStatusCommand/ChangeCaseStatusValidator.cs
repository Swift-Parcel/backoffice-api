using FluentValidation;
using SwiftParcel.Application.Cases.Commands.ChangeCaseStatusCommand;

namespace SwiftParcel.Application.Cases.Commands.UpdateCaseStatusCommand;

public class ChangeCaseStatusValidator : AbstractValidator<ChangeStatusCommand>
{
    public ChangeCaseStatusValidator()
    {
        RuleFor(x => x.CaseNumber)
            .NotEmpty()
            .WithMessage("Case number is required.");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid status provided.");
    }
}