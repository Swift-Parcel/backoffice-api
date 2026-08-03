using FluentValidation;

namespace SwiftParcel.Application.Cases.Queries.GetCaseNotes;

public class GetCaseNotesQueryValidator : AbstractValidator<GetCaseNotesQuery>
{
    public GetCaseNotesQueryValidator()
    {
        RuleFor(x => x.CaseNumber).NotEmpty().WithMessage("Case number is required.");
    }
}