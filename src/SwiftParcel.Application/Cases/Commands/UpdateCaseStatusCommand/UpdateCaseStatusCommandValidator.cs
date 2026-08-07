using FluentValidation;

namespace SwiftParcel.Application.Cases.Commands.UpdateCaseStatusCommand;

public class UpdateCaseStatusCommandValidator : AbstractValidator<UpdateCaseStatusCommand>
{
    public UpdateCaseStatusCommandValidator()
    {
        RuleFor(x => x.CaseNumber)
            .NotEmpty()
            .WithMessage("Case number is required.");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid status provided.");
    }
}