using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.AddCaseFeedback;

public class AddCaseFeedbackCommandHandler : IRequestHandler<AddCaseFeedbackCommand, Result<bool>>
{
    private readonly ICaseRepository _caseRepository;

    public AddCaseFeedbackCommandHandler(ICaseRepository caseRepository) => _caseRepository = caseRepository;

    public async Task<Result<bool>> Handle(AddCaseFeedbackCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await _caseRepository.GetByCaseNumberAsync(request.CaseNumber, cancellationToken);
            
        if (caseEntity == null)
        {
            return Result<bool>.Failure(Error.NotFound(
                "add_feedback__case_not_found", 
                "Case not found."));
        }
        
        caseEntity.SatisfactionScore = request.Score;
        caseEntity.UpdatedDate = DateTime.UtcNow;
        
        await _caseRepository.UpdateAsync(caseEntity, cancellationToken);
        
        return Result<bool>.Success(true);
    }
}