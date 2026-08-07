using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Common.Interfaces.Repositories;

using SwiftParcel.Domain.Entities;

public interface IParcelRepository
{
    Task<Parcel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Parcel>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<Parcel?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Parcel parcel, CancellationToken cancellationToken = default);
    Task UpdateAsync(Parcel parcel, CancellationToken cancellationToken = default);
    Task DeleteAsync(Parcel parcel, CancellationToken cancellationToken = default);
    Task<List<CustomerParcelDto>> GetCustomerParcelsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);
    Task<List<Parcel>> GetByTrackingNumbersAsync(IEnumerable<string> trackingNumbers, CancellationToken cancellationToken = default);
    Task<ParcelStatus?> GetStatusByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);
}