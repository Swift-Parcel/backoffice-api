using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCaseNotes;

public class GetCaseNotesQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCaseNotesQuery, Result<IReadOnlyList<CaseNoteDto>>>
{
    public async Task<Result<IReadOnlyList<CaseNoteDto>>> Handle(GetCaseNotesQuery request, CancellationToken cancellationToken)
    {
        var caseExists = await context.Cases
            .AnyAsync(c => c.CaseNumber == request.CaseNumber, cancellationToken);
            
        if (!caseExists)
        {
            return Result<IReadOnlyList<CaseNoteDto>>.Failure(Error.NotFound(
                "get_notes__case_not_found", 
                $"Case with number {request.CaseNumber} was not found."));
        }

        var notes = await context.CaseNotes
            .AsNoTracking()
            .Where(n => n.Case.CaseNumber == request.CaseNumber)
            .OrderBy(n => n.CreatedDate)
            .Select(n => new CaseNoteDto(n.CreatedDate, n.NoteText))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<CaseNoteDto>>.Success(notes);
    }
}