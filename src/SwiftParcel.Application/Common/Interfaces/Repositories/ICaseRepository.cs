namespace SwiftParcel.Application.Common.Interfaces.Repositories;

using SwiftParcel.Domain.Entities;

public interface ICaseRepository
{
    Task AddAsync(Case newCase, CancellationToken cancellationToken = default);
}