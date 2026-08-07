using FluentValidation;

namespace SwiftParcel.Application.Cases.Queries.GetCustomerCases;

public class GetCustomerCasesQueryValidator : AbstractValidator<GetCustomerCasesQuery>
{
    public GetCustomerCasesQueryValidator()
    {
        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Customer email is required.")
            .EmailAddress().WithMessage("Must be a valid email address.");
    }
}