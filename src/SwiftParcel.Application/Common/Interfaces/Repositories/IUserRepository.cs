using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Common.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdWithRegionsAsync(int userId, CancellationToken cancellationToken = default);
}