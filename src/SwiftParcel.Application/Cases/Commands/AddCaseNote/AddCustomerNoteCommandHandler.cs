using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Entities;

public class AddCustomerNoteCommandHandler : IRequestHandler<AddCustomerNoteCommand, Result<int>>
{
    private readonly IAppDbContext _context;
    public AddCustomerNoteCommandHandler(IAppDbContext context) => _context = context;

    public async Task<Result<int>> Handle(AddCustomerNoteCommand request, CancellationToken ct)
    {
        var caseEntity = await _context.Cases
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.CaseNumber == request.CaseNumber, ct);
            
        if (caseEntity == null)
            return Result<int>.Failure(Error.NotFound("case_not_found", "Case not found."));

        if (caseEntity.Customer.Email != request.CustomerEmail)
            return Result<int>.Failure(Error.Conflict("unauthorized_customer", "Customer email does not match case owner."));

        var note = new CaseNote
        {
            CaseId = caseEntity.Id,
            NoteText = request.Message,
            CreatedDate = DateTime.UtcNow,
            IsInternal = false,
            CustomerId = caseEntity.CustomerId,
            HandlerId = null,
            Attachment = request.Attachment
        };

        caseEntity.Notes.Add(note);
        caseEntity.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        
        return Result<int>.Success(note.Id);
    }
}