using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCustomerCaseNotes;

public class GetCustomerCaseNotesQueryHandler : IRequestHandler<GetCustomerCaseNotesQuery, Result<IReadOnlyList<CustomerFacingCaseNoteDto>>>
{
    private readonly IAppDbContext _context;

    public GetCustomerCaseNotesQueryHandler(IAppDbContext context) => _context = context;

    public async Task<Result<IReadOnlyList<CustomerFacingCaseNoteDto>>> Handle(GetCustomerCaseNotesQuery request, CancellationToken cancellationToken)
    {
        var caseExists = await _context.Cases
            .AnyAsync(c => c.CaseNumber == request.CaseNumber, cancellationToken);
            
        if (!caseExists)
        {
            return Result<IReadOnlyList<CustomerFacingCaseNoteDto>>.Failure(Error.NotFound(
                "get_customer_notes__case_not_found", 
                $"Case with number {request.CaseNumber} was not found."));
        }

        var notes = await _context.CaseNotes
            .AsNoTracking()
            .Where(n => n.Case.CaseNumber == request.CaseNumber && !n.IsInternal)
            .OrderBy(n => n.CreatedDate)
            .Select(n => new CustomerFacingCaseNoteDto(
                n.CreatedDate,
                n.NoteText))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<CustomerFacingCaseNoteDto>>.Success(notes);
    }
}