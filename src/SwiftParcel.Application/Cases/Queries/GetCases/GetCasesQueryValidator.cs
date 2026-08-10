using FluentValidation;

namespace SwiftParcel.Application.Cases.Queries.GetCases;

public class GetCasesQueryValidator : AbstractValidator<GetCasesQuery>
{
    public GetCasesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("A page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        When(x => !string.IsNullOrEmpty(x.CustomerEmail), () =>
        {
            RuleFor(x => x.CustomerEmail)
                .EmailAddress()
                .WithMessage("Invalid email format.");
        });
    }
}