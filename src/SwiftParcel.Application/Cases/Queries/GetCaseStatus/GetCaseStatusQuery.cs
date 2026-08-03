using FluentValidation;
using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCaseStatus;

public record GetCaseStatusQuery(string CaseNumber) : IRequest<Result<CaseStatusResponse>>;

public class GetCaseStatusQueryValidator : AbstractValidator<GetCaseStatusQuery>
{
    public GetCaseStatusQueryValidator()
    {
        RuleFor(x => x.CaseNumber).NotEmpty().WithMessage("Case number is required.");
    }
}