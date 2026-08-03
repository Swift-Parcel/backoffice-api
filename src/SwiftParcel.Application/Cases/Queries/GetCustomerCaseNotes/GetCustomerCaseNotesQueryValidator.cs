using FluentValidation;
using SwiftParcel.Application.Cases.Queries.GetCustomerCaseNotes;

public class GetCustomerCaseNotesQueryValidator : AbstractValidator<GetCustomerCaseNotesQuery>
{
    public GetCustomerCaseNotesQueryValidator()
    {
        RuleFor(x => x.CaseNumber).NotEmpty().WithMessage("Case number is required.");
    }
}