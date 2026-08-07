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
}