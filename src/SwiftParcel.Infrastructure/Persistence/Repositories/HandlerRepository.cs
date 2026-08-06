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
}