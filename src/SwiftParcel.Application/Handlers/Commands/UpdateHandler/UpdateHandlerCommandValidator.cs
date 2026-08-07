using FluentValidation;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandler;

public class UpdateHandlerCommandValidator : AbstractValidator<UpdateHandlerCommand>
{
    public UpdateHandlerCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Department != null || x.MaxCases > 0)
            .WithMessage("At least one field (Department or MaxCases) must be provided to update.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department cannot be empty if provided.")
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters.")
            .When(x => x.Department != null);

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Handler ID is required.");

        RuleFor(x => x.MaxCases)
            .GreaterThan(0).WithMessage("MaxCases must be greater than zero.")
            .When(x => x.MaxCases > 0);
    }
}