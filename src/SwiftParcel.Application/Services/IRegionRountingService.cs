using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Services;

public interface IRegionRoutingService
{
    Task<int> DetermineRegionAsync(Case caseEntity, CancellationToken cancellationToken = default);
}