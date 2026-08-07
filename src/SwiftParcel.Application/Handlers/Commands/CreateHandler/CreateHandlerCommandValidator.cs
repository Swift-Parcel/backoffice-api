using FluentValidation;

namespace SwiftParcel.Application.Handlers.Commands.CreateHandler;

public class CreateHandlerCommandValidator : AbstractValidator<CreateHandlerCommand>
{
    public CreateHandlerCommandValidator()
    {
        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters.");

        RuleFor(x => x.MaxCases)
            .GreaterThan(0).WithMessage("MaxCases must be greater than zero.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID is required.");
    }
}