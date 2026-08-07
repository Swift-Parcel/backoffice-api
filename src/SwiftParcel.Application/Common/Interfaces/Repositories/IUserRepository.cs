using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Common.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdWithRegionsAsync(int userId, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);
}