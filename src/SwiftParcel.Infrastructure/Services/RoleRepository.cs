using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Infrastructure.Persistence;

namespace SwiftParcel.Infrastructure.Services;

public class RoleRepository(AppDbContext dbContext) : IRoleRepository
{
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Roles.AnyAsync(r => r.Id == id, cancellationToken);
    }
}