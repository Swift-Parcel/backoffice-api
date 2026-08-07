using FluentValidation;

namespace SwiftParcel.Application.Cases.Queries.GetCaseStatus;

public class GetCaseStatusQueryValidator : AbstractValidator<GetCaseStatusQuery>
{
    public GetCaseStatusQueryValidator()
    {
        RuleFor(x => x.CaseNumber).NotEmpty().WithMessage("Case number is required.");
    }
}