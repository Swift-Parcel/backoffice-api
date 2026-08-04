using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Infrastructure.Persistence;

public class CaseAssignmentService : ICaseAssignmentService
{
    private readonly AppDbContext _dbContext;

    private CaseStatus[] activeStatuses = new[] 
    { 
        CaseStatus.Open, 
        CaseStatus.InProgress, 
        CaseStatus.Escalated, 
        CaseStatus.AwaitingCustomer 
    };
    
    public CaseAssignmentService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AssignCaseAsync(int caseId, int handlerId, CancellationToken cancellationToken = default)
    {
        var ticket = await _dbContext.Cases
            .FirstOrDefaultAsync(c => c.Id == caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Case with ID {caseId} was not found.");

        if (ticket.HandlerId == handlerId)
        {
            return; 
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var handler = await _dbContext.Handlers
                .FromSqlInterpolated($"SELECT * FROM handlers WHERE id = {handlerId} FOR UPDATE")
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw new KeyNotFoundException($"Handler with ID {handlerId} was not found.");

            
            var activeCasesCount = await _dbContext.Cases
                .CountAsync(c => c.HandlerId == handlerId && 
                                 activeStatuses.Contains(c.Status), 
                    cancellationToken);
            
            if (activeCasesCount >= handler.MaxCases)
            {
                throw new CapacityExceededException(handler.Id, handler.MaxCases);
            }

            ticket.HandlerId = handlerId;

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}