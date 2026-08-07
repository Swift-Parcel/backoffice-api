using FluentValidation;

namespace SwiftParcel.Application.Cases.Commands.AddCaseFeedback;

public class AddCaseFeedbackCommandValidator : AbstractValidator<AddCaseFeedbackCommand>
{
    public AddCaseFeedbackCommandValidator()
    {
        RuleFor(x => x.CaseNumber).NotEmpty();
        RuleFor(x => x.Score)
            .InclusiveBetween(1, 5)
            .WithMessage("Score must be between 1 and 5.");
    }
}