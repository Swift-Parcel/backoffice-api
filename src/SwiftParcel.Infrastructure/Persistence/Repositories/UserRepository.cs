using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Extensions;

namespace SwiftParcel.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdWithRegionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.Regions)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }
    
    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken = default)
    {
        return !await _dbContext.Users.AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
    {
        return !await _dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }
    
    public async Task<User?> GetByIdWithRegionsForUpdateAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.Regions)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }
    
    public async Task<List<User>> GetFilteredWithRegionsAsync(
        int? roleId, 
        bool? isActive, 
        string? searchTerm, 
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Regions)
            .AsQueryable();
        
        query = ApplyFilters(query, roleId, isActive, searchTerm);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<PagedList<User>> GetPagedFilteredWithRegionsAsync(int? roleId, bool? isActive, string? searchTerm, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Users
            .Include(u => u.Regions)
            .AsNoTracking();
        
        query = ApplyFilters(query, roleId, isActive, searchTerm);
        
        query = query.OrderBy(u => u.Username);
        
        return await query
            .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }
    
    private static IQueryable<User> ApplyFilters(IQueryable<User> query, int? roleId, bool? isActive, string? searchTerm)
    {
        if (roleId.HasValue)
        {
            query = query.Where(u => u.RoleId == roleId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(u => 
                u.FullName.ToLower().Contains(term) || 
                u.Email.ToLower().Contains(term) ||
                u.Username.ToLower().Contains(term));
        }

        return query;
    }
}