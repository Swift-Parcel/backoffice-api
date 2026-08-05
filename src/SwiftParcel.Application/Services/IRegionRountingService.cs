using SwiftParcel.Domain.Entities;

public interface IRegionRoutingService
{
    Task<int> DetermineRegionAsync(Case caseEntity, CancellationToken cancellationToken = default);
}