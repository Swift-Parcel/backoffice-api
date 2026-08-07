using FluentValidation;

namespace SwiftParcel.Application.Cases.Commands.AddCaseNote;

public class AddCustomerNoteCommandValidator : AbstractValidator<AddCustomerNoteCommand>
{
    public AddCustomerNoteCommandValidator()
    {
        RuleFor(x => x.CaseNumber).NotEmpty();
        RuleFor(x => x.Message).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CustomerEmail).NotEmpty().EmailAddress();
    }
}