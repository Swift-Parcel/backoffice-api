using FluentValidation;

namespace SwiftParcel.Application.Customers.Queries.GetCustomerByEmail;

public class GetCustomerByEmailQueryValidator : AbstractValidator<GetCustomerByEmailQuery>
{
    public GetCustomerByEmailQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}