using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Common.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<(List<Role> Items, int TotalCount)> GetPagedAsync(
        string? nameFilter, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);
}