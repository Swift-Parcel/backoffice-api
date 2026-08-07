using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.AddCaseNote;

public class AddCustomerNoteCommandHandler : IRequestHandler<AddCustomerNoteCommand, Result<int>>
{
    private readonly ICaseRepository _caseRepository;
    
    public AddCustomerNoteCommandHandler(ICaseRepository caseRepository) => _caseRepository = caseRepository;

    public async Task<Result<int>> Handle(AddCustomerNoteCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await _caseRepository.GetByCaseNumberWithCustomerAsync(request.CaseNumber, cancellationToken);
            
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

        await _caseRepository.UpdateAsync(caseEntity, cancellationToken);
        
        return Result<int>.Success(note.Id);
    }
}