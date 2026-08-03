using FluentValidation;

public class AddHandlerNoteCommandValidator : AbstractValidator<AddHandlerNoteCommand>
{
    public AddHandlerNoteCommandValidator()
    {
        RuleFor(x => x.CaseNumber).NotEmpty();
        RuleFor(x => x.Message).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.HandlerId).GreaterThan(0);
    }
}