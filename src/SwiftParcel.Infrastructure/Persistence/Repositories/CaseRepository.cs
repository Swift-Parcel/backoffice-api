using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Infrastructure.Persistence.Repositories;

public class CaseRepository : ICaseRepository
{
    public Task AddAsync(Case newCase, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}