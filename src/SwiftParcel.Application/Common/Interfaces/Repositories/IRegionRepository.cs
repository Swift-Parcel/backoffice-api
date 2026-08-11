using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Common.Interfaces.Repositories;

public interface IRegionRepository
{
    Task<int> GetActiveRegionIdByCountryCodeAsync(string countryCode, CancellationToken cancellationToken = default);
    Task<bool> IsActiveAsync(int regionId, CancellationToken cancellationToken = default);
    Task<List<Region>> GetByIdsAsync(IEnumerable<int> regionIds, CancellationToken cancellationToken = default);
    Task<(List<Region> Items, int TotalCount)> GetPagedAsync(
        string? nameFilter, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);
}