using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCustomerCaseNotes;

public class GetCustomerCaseNotesQueryHandler : IRequestHandler<GetCustomerCaseNotesQuery, Result<IReadOnlyList<CustomerFacingCaseNoteDto>>>
{
    private readonly ICaseRepository _caseRepository;

    public GetCustomerCaseNotesQueryHandler(ICaseRepository caseRepository) => _caseRepository = caseRepository;

    public async Task<Result<IReadOnlyList<CustomerFacingCaseNoteDto>>> Handle(GetCustomerCaseNotesQuery request, CancellationToken cancellationToken)
    {
        var caseExists = await _caseRepository.ExistsByCaseNumberAsync(request.CaseNumber, cancellationToken);
            
        if (!caseExists)
        {
            return Result<IReadOnlyList<CustomerFacingCaseNoteDto>>.Failure(Error.NotFound(
                "get_customer_notes__case_not_found", 
                $"Case with number {request.CaseNumber} was not found."));
        }

        var notes = await _caseRepository.GetCustomerCaseNotesAsync(request.CaseNumber, cancellationToken);

        return Result<IReadOnlyList<CustomerFacingCaseNoteDto>>.Success(notes);
    }
}