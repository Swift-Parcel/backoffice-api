using FluentValidation;

namespace SwiftParcel.Application.Regions.Queries.GetRegions;

public class GetRegionsQueryValidator : AbstractValidator<GetRegionsQuery>
{
    public GetRegionsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100.");
    }
}