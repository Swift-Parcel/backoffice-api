using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Common.Interfaces.Repositories;

public interface IHandlerRepository
{
    Task<Handler?> GetWithLockAndCasesAsync(int handlerId, CancellationToken cancellationToken = default);
    Task AddAsync(Handler handler, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}