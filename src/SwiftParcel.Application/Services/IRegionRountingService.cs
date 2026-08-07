using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Services;

public interface IRegionRoutingService
{
    Task<int> DetermineRegionAsync(Case caseEntity, CancellationToken cancellationToken = default);

    Task<int> DetermineRegionAsync(IEnumerable<Parcel> parcels,
        CancellationToken cancellationToken = default);

    Task<int> DetermineRegionAsync(Parcel parcel,
        CancellationToken cancellationToken = default);
}