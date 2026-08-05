using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCustomerCaseNotes;

public class GetCustomerCaseNotesQueryHandler : IRequestHandler<GetCustomerCaseNotesQuery, Result<IReadOnlyList<CaseNoteDto>>>
{
    private readonly IAppDbContext _context;

    public GetCustomerCaseNotesQueryHandler(IAppDbContext context) => _context = context;

    public async Task<Result<IReadOnlyList<CaseNoteDto>>> Handle(GetCustomerCaseNotesQuery request, CancellationToken cancellationToken)
    {
        var caseExists = await _context.Cases
            .AnyAsync(c => c.CaseNumber == request.CaseNumber, cancellationToken);
            
        if (!caseExists)
        {
            return Result<IReadOnlyList<CaseNoteDto>>.Failure(Error.NotFound(
                "get_customer_notes__case_not_found", 
                $"Case with number {request.CaseNumber} was not found."));
        }

        var notes = await _context.CaseNotes
            .AsNoTracking()
            .Where(n => n.Case.CaseNumber == request.CaseNumber && !n.IsInternal)
            .OrderBy(n => n.CreatedDate)
            .Select(n => new CaseNoteDto(
                n.CreatedDate,
                n.NoteText, 
                n.HandlerId, 
                n.Handler!.FullName, 
                n.CustomerId, 
                n.Customer!.FullName,  
                n.Attachment))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<CaseNoteDto>>.Success(notes);
    }
}