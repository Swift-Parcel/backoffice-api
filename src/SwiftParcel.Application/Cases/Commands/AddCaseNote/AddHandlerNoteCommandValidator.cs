using FluentValidation;

namespace SwiftParcel.Application.Cases.Commands.AddCaseNote;

public class AddHandlerNoteCommandValidator : AbstractValidator<AddHandlerNoteCommand>
{
    public AddHandlerNoteCommandValidator()
    {
        RuleFor(x => x.CaseNumber)
            .NotEmpty().WithMessage("Case number is required.")
            .MaximumLength(20).WithMessage("Case number cannot exceed 20 characters.");
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Note message is required.")
            .MaximumLength(2000)
            .WithMessage("Note message is required and cannot exceed 2000 characters.");
    }
}