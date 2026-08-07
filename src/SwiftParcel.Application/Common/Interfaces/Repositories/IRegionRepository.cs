namespace SwiftParcel.Application.Common.Interfaces.Repositories;

public interface IRegionRepository
{
    Task<int> GetActiveRegionIdByCountryCodeAsync(string countryCode, CancellationToken cancellationToken = default);
}