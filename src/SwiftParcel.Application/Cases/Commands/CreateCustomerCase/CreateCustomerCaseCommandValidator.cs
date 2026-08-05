using FluentValidation;
using SwiftParcel.Application.Cases.Commands.CreateCustomerCase;

namespace SwiftParcel.Application.Integration.Validators;

public class CreateCustomerCaseCommandValidator : AbstractValidator<CreateCustomerCaseCommand>
{
    public CreateCustomerCaseCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(5).WithMessage("Title must be at least 5 characters long.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");
        
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters long.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");
        
        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Customer email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
        
        RuleFor(x => x.CaseType)
            .IsInEnum().WithMessage("Invalid case type.");
        
        RuleFor(x => x.TrackingNumbers)
            .NotNull().WithMessage("Tracking numbers list cannot be null.");

        RuleForEach(x => x.TrackingNumbers)
            .NotEmpty().WithMessage("Tracking numbers cannot be empty strings.");
    }
}