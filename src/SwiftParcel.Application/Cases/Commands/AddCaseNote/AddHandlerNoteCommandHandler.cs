using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Entities;

public class AddHandlerNoteCommandHandler : IRequestHandler<AddHandlerNoteCommand, Result<int>>
{
    private readonly IAppDbContext _context;
    public AddHandlerNoteCommandHandler(IAppDbContext context) => _context = context;

    public async Task<Result<int>> Handle(AddHandlerNoteCommand request, CancellationToken ct)
    {
        var caseEntity = await _context.Cases
            .FirstOrDefaultAsync(c => c.CaseNumber == request.CaseNumber, ct);
            
        if (caseEntity == null)
            return Result<int>.Failure(Error.NotFound("case_not_found", "Case not found."));

        var note = new CaseNote
        {
            CaseId = caseEntity.Id,
            NoteText = request.Message,
            CreatedDate = DateTime.UtcNow,
            IsInternal = request.IsInternal,
            HandlerId = request.HandlerId,
            CustomerId = null,
            Attachment = request.Attachment
        };

        caseEntity.Notes.Add(note);
        caseEntity.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        
        return Result<int>.Success(note.Id);
    }
}