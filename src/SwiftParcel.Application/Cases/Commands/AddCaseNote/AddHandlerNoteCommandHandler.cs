using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.AddCaseNote;

public class AddHandlerNoteCommandHandler : IRequestHandler<AddHandlerNoteCommand, Result<int>>
{
    private readonly ICaseRepository _caseRepository;
    private readonly IHandlerRepository _handlerRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddHandlerNoteCommandHandler(
        ICaseRepository caseRepository,
        IHandlerRepository handlerRepository,
        ICurrentUserService currentUserService)
    {
        _caseRepository = caseRepository;
        _handlerRepository = handlerRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(AddHandlerNoteCommand request, CancellationToken cancellationToken)
    {
        int? handlerId = null;

        if (_currentUserService.UserId.HasValue)
        {
            var handler = await _handlerRepository.GetByUserIdWithDetailsAsync(_currentUserService.UserId.Value, cancellationToken);
            handlerId = handler?.Id;
        }

        var caseEntity = await _caseRepository.GetByCaseNumberAsync(request.CaseNumber, cancellationToken);

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

        await _caseRepository.UpdateAsync(caseEntity, cancellationToken);

        return Result<int>.Success(note.Id);
    }
}