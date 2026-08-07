namespace SwiftParcel.Application.Common.Interfaces.Repositories;

public interface IRegionRepository
{
    Task<int> GetActiveRegionIdByCountryCodeAsync(string countryCode, CancellationToken cancellationToken = default);
    Task<bool> IsActiveAsync(int regionId, CancellationToken cancellationToken = default);
}