using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Common.Interfaces.Repositories;

public interface IHandlerRepository
{
    Task<Handler?> GetWithLockAndCasesAsync(int handlerId, CancellationToken cancellationToken = default);
    Task AddAsync(Handler handler, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    
    Task<Handler?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Handler handler, CancellationToken cancellationToken = default);
    Task<Handler?> GetByIdWithUserRegionsAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetActiveCasesCountAsync(int handlerId, CancellationToken cancellationToken = default);
    Task<Handler?> GetByUserIdWithDetailsAsync(int userId, CancellationToken cancellationToken = default);
    Task<Handler?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Handler>> GetFilteredWithDetailsAsync(
        IEnumerable<int>? allowedRegionIds, 
        bool? isActive, 
        string? department, 
        CancellationToken cancellationToken = default);
}