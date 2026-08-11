using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
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
            .FromSql($"SELECT * FROM \"handlers\" WHERE \"id\" = {handlerId} FOR UPDATE")
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
    
    public async Task<Handler?> GetByUserIdWithDetailsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Handlers
            .Include(h => h.User)
            .ThenInclude(u => u.Regions)
            .Include(h => h.Cases.Where(c => Case.ActiveStatuses.Contains(c.Status))) 
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.UserId == userId, cancellationToken);
    }
    
    public async Task<Handler?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Handlers
            .Include(h => h.User)
            .ThenInclude(u => u.Regions)
            .Include(h => h.Cases.Where(c => Case.ActiveStatuses.Contains(c.Status))) 
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }
    
    public async Task<PagedList<Handler>> GetFilteredPagedWithDetailsAsync(
        IEnumerable<int>? allowedRegionIds, 
        bool? isActive, 
        string? department, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Handlers
            .Include(h => h.User)
            .ThenInclude(u => u.Regions)
            .Include(h => h.Cases)
            .AsQueryable();

        if (allowedRegionIds != null)
        {
            query = query.Where(h => h.User.Regions.Any(r => allowedRegionIds.Contains(r.Id)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(h => h.IsActive == isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(department))
        {
            query = query.Where(h => h.Department == department);
        }

        var totalCount = await query.CountAsync(cancellationToken);
    
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<Handler>(items, totalCount, pageNumber, pageSize);
    }
    
    public async Task<int?> GetIdByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var handler = await _dbContext.Handlers
            .Where(h => h.UserId == userId)
            .Select(h => new { h.Id })
            .FirstOrDefaultAsync(cancellationToken);

        return handler?.Id;
    }
}