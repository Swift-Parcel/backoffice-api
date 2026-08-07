using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.AddCaseNote;

public class AddHandlerNoteCommandHandler : IRequestHandler<AddHandlerNoteCommand, Result<int>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddHandlerNoteCommandHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(AddHandlerNoteCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        
        var handlerId = await _context.Handlers
            .Where(h => h.UserId == currentUserId)  
            .Select(h => h.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        var caseEntity = await _context.Cases
            .FirstOrDefaultAsync(c => c.CaseNumber == request.CaseNumber, cancellationToken);

        if (caseEntity == null)
            return Result<int>.Failure(Error.NotFound("case_not_found", "Case not found."));

        var note = new CaseNote
        {
            CaseId = caseEntity.Id,
            NoteText = request.Message,
            CreatedDate = DateTime.UtcNow,
            IsInternal = request.IsInternal,
            HandlerId = handlerId,
            CustomerId = null,
            Attachment = request.Attachment
        };

        caseEntity.Notes.Add(note);
        caseEntity.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(note.Id);
    }
}