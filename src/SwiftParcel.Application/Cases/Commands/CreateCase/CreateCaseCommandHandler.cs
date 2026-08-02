using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Cases.Commands.CreateCase;

public class CreateCaseCommandHandler : IRequestHandler<CreateCaseCommand, Result<int>>
{
    private readonly IAppDbContext _context;
    
    public CreateCaseCommandHandler(IAppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<int>> Handle(CreateCaseCommand request, CancellationToken cancellationToken)
    {
        if (request.ParcelIds.Count == 0)
        {
            return Result<int>.Failure(Error.Validation("ParcelIds.Empty", "At least one parcel has to be given."));
        }
        
        var existingParcelIds = await _context.Parcels
            .Where(p => request.ParcelIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);
        
        var missingParcelIds = request.ParcelIds
            .Except(existingParcelIds)
            .ToList();
        
        if(missingParcelIds.Any())
        {
            var missingIdsString = string.Join(", ", missingParcelIds);
            
            return Result<int>.Failure(Error.Validation(
                "create_case__missing_parcels", 
                $"The following ParcelIds do not exist: {missingIdsString}"));
        }

        // 2. SLA kiszámítása a CaseType alapján (pl. LOST = 48h)
        // 3. Új Case entitás létrehozása & mentése

        var createdCaseId = 23;
        
        return Result<int>.Success(createdCaseId);
    }
}