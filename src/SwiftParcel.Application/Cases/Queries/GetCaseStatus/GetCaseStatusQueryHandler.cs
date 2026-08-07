using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCaseStatus;

public class GetCaseStatusQueryHandler : IRequestHandler<GetCaseStatusQuery, Result<CaseStatusResponse>>
{
    private readonly ICaseRepository _caseRepository;

    public GetCaseStatusQueryHandler(ICaseRepository caseRepository) => _caseRepository = caseRepository;

    public async Task<Result<CaseStatusResponse>> Handle(GetCaseStatusQuery request, CancellationToken cancellationToken)
    {
        var response = await _caseRepository.GetCaseStatusAsync(request.CaseNumber, cancellationToken);

        if (response == null)
        {
            return Result<CaseStatusResponse>.Failure(Error.NotFound(
                "get_case_status__not_found", 
                $"Case with number {request.CaseNumber} was not found."));
        }

        return Result<CaseStatusResponse>.Success(response);
    }
}