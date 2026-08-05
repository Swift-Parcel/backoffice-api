using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Cases.Queries.GetCaseStatus;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

public class GetCaseStatusQueryHandler : IRequestHandler<GetCaseStatusQuery, Result<CaseStatusResponse>>
{
    private readonly IAppDbContext _context;

    public GetCaseStatusQueryHandler(IAppDbContext context) => _context = context;

    public async Task<Result<CaseStatusResponse>> Handle(GetCaseStatusQuery request, CancellationToken cancellationToken)
    {
        var caseEntity = await _context.Cases
            .Include(c => c.Notes)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CaseNumber == request.CaseNumber, cancellationToken);
            
        if (caseEntity == null)
        {
            return Result<CaseStatusResponse>.Failure(Error.NotFound(
                "get_case_status__not_found", 
                $"Case with number {request.CaseNumber} was not found."));
        }
        
        var notesDto = caseEntity.Notes
            .Where(n => !n.IsInternal)
            .OrderBy(n => n.CreatedDate)
            .Select(n => new CaseNoteDto(
                n.CreatedDate,
                n.NoteText, 
                n.HandlerId, 
                n.Handler!.FullName, 
                n.CustomerId, 
                n.Customer!.FullName,  
                n.Attachment))
            .ToList();
            
        var response = new CaseStatusResponse(caseEntity.Status, notesDto, caseEntity.Resolution);
        
        return Result<CaseStatusResponse>.Success(response);
    }
}