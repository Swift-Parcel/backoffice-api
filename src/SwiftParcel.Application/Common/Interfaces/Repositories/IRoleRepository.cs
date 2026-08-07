namespace SwiftParcel.Application.Common.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}