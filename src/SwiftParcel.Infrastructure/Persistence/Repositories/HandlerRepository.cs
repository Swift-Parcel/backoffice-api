using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Infrastructure.Persistence.Repositories;

public class HandlerRepository : IHandlerRepository
{
    private readonly AppDbContext _dbContext;

    public HandlerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Handler?> GetWithLockAndCasesAsync(int handlerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Handlers
            .FromSql($"SELECT * FROM \"handlers\" WHERE \"Id\" = {handlerId} FOR UPDATE")
            .Include(h => h.Cases)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task AddAsync(Handler handler, CancellationToken cancellationToken = default)
    {
        await _dbContext.Handlers.AddAsync(handler, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Handlers.AnyAsync(h => h.UserId == userId, cancellationToken);
    }
    
    public async Task<Handler?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Handlers.FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Handler handler, CancellationToken cancellationToken = default)
    {
        _dbContext.Handlers.Update(handler);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Handler?> GetByIdWithUserRegionsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Handlers
            .Include(h => h.User)
            .ThenInclude(u => u.Regions)
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<int> GetActiveCasesCountAsync(int handlerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cases
            .CountAsync(c => c.HandlerId == handlerId && 
                             Case.ActiveStatuses.Contains(c.Status), 
                cancellationToken);
    }
}