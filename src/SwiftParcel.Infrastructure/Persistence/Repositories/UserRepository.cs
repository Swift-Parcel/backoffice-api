using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Domain.Entities;

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
}