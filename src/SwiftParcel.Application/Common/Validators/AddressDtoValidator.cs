using FluentValidation;

namespace SwiftParcel.Application.Common.Validators;

public class AddressDtoValidator: AbstractValidator<DTO.AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required.")
            .MaximumLength(100).WithMessage("Street cannot exceed 100 characters.");

        RuleFor(x => x.StreetNumber)
            .NotEmpty().WithMessage("Street number is required.")
            .MaximumLength(20).WithMessage("Street number cannot exceed 20 characters.");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code is required.");

        RuleFor(x => x.CountryCode)
            .NotEmpty().WithMessage("Country code is required.")
            .Length(2).WithMessage("Country code must be exactly 2 characters.");
    }
}