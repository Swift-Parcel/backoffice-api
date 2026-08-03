using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Cases.Commands.AddCaseFeedback;

public class AddCaseFeedbackCommandHandler : IRequestHandler<AddCaseFeedbackCommand, Result<bool>>
{
    private readonly IAppDbContext _context;

    public AddCaseFeedbackCommandHandler(IAppDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(AddCaseFeedbackCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await _context.Cases
            .FirstOrDefaultAsync(c => c.CaseNumber == request.CaseNumber, cancellationToken);
            
        if (caseEntity == null)
        {
            return Result<bool>.Failure(Error.NotFound(
                "add_feedback__case_not_found", 
                "Case not found."));
        }
        
        caseEntity.SatisfactionScore = request.Score;
        caseEntity.UpdatedDate = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result<bool>.Success(true);
    }
}