using FluentValidation;
using SwiftParcel.Application.Common.Validators;

namespace SwiftParcel.Application.Customers.Commands.CreatePortalCustomer;

public class CreatePortalCustomerCommandValidator : AbstractValidator<CreatePortalCustomerCommand>
{
    public CreatePortalCustomerCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required.")
            .Matches(@"^\+?[1-9]\d{7,14}$").WithMessage("Invalid phone number format.");
        
        RuleFor(x => x.AddressDto)
            .NotNull().WithMessage("Address is required.")
            .SetValidator(new AddressDtoValidator());
    }
}