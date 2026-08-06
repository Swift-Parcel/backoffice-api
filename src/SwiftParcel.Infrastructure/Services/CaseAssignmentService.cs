using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Exceptions;
using SwiftParcel.Infrastructure.Persistence;

namespace SwiftParcel.Infrastructure.Services;

public class CaseAssignmentService : ICaseAssignmentService
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    
    public CaseAssignmentService(AppDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task AssignCaseAsync(string caseNumber, int handlerId, CancellationToken cancellationToken = default)    
    {
        var ticket = await _dbContext.Cases
                         .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber, cancellationToken)
                     ?? throw new ResourceNotFoundException("Case", caseNumber);

        if (!_currentUser.HasAccessToRegion(ticket.RegionId))
        {
            throw new ForbiddenAccessException("You do not have permission to assign cases in this region.");
        }

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

            var targetUser = await _dbContext.Users
                .Include(u => u.Regions)
                .FirstOrDefaultAsync(u => u.Id == handler.UserId, cancellationToken);
            
            if (targetUser == null)
            {
                throw new KeyNotFoundException($"User associated with Handler {handlerId} was not found.");
            }

            if (!targetUser.IsActive)
            {
                throw new BusinessRuleValidationException(
                    "handler_deactivated", 
                    "The selected handler's user account has been deactivated. They cannot be assigned new cases.");
            }

            bool targetHandlerIsInRegion = targetUser.Regions.Any(r => r.Id == ticket.RegionId);
            
            if (!targetHandlerIsInRegion && !_currentUser.CanAccessAllRegions)
            {
                throw new BusinessRuleValidationException(
                    "invalid_handler_region", 
                    "The selected handler does not operate in this case's region.");
            }

            if (ticket.IsEscalated && handler.Department != "Escalations")
            {
                throw new BusinessRuleValidationException(
                    "invalid_department", 
                    "Escalated cases must be assigned to the Escalations department.");
            }

            if (!ticket.IsEscalated && ticket.CaseType == CaseType.Lost && handler.Department != "Investigations")
            {
                throw new BusinessRuleValidationException(
                    "invalid_department", 
                    "Lost parcels must be assigned to the Investigations department.");
            }

            if (!ticket.IsEscalated && ticket.CaseType != CaseType.Lost && handler.Department != "Customer Support")
            {
                throw new BusinessRuleValidationException(
                    "invalid_department", 
                    "Standard support cases must be assigned to Customer Support.");
            }
            
            var activeCasesCount = await _dbContext.Cases
                .CountAsync(c => c.HandlerId == handlerId && 
                                 Case.ActiveStatuses.Contains(c.Status),
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