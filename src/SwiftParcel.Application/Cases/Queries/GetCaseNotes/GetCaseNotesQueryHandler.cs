using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Queries.GetCaseNotes;

public class GetCaseNotesQueryHandler(ICaseRepository caseRepository)
    : IRequestHandler<GetCaseNotesQuery, Result<PagedList<CaseNoteDto>>>
{
    public async Task<Result<PagedList<CaseNoteDto>>> Handle(GetCaseNotesQuery request, CancellationToken cancellationToken)
    {
        var caseExists = await caseRepository.ExistsByCaseNumberAsync(request.CaseNumber, cancellationToken);
            
        if (!caseExists)
        {
            return Result<PagedList<CaseNoteDto>>.Failure(Error.NotFound($"Case with number {request.CaseNumber} was not found."));
        }

        var notes = await caseRepository.GetPagedCaseNotesAsync(request.CaseNumber, request.PageNumber, request.PageSize, cancellationToken);

        return Result<PagedList<CaseNoteDto>>.Success(notes);
    }
}