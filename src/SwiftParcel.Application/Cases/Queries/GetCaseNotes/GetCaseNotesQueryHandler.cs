using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Queries.GetCaseNotes;

public class GetCaseNotesQueryHandler(ICaseRepository caseRepository)
    : IRequestHandler<GetCaseNotesQuery, Result<IReadOnlyList<CaseNoteDto>>>
{
    public async Task<Result<IReadOnlyList<CaseNoteDto>>> Handle(GetCaseNotesQuery request, CancellationToken cancellationToken)
    {
        var caseExists = await caseRepository.ExistsByCaseNumberAsync(request.CaseNumber, cancellationToken);
            
        if (!caseExists)
        {
            return Result<IReadOnlyList<CaseNoteDto>>.Failure(Error.NotFound(
                "get_notes__case_not_found", 
                $"Case with number {request.CaseNumber} was not found."));
        }

        var notes = await caseRepository.GetCaseNotesAsync(request.CaseNumber, cancellationToken);

        return Result<IReadOnlyList<CaseNoteDto>>.Success(notes);
    }
}