using SwiftParcel.Application.DTO.Parcels;

namespace SwiftParcel.Application.Common.Interfaces.Repositories;

using SwiftParcel.Domain.Entities;

public interface IParcelRepository
{
    Task<Parcel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Parcel?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Parcel parcel, CancellationToken cancellationToken = default);
    Task UpdateAsync(Parcel parcel, CancellationToken cancellationToken = default);
    Task DeleteAsync(Parcel parcel, CancellationToken cancellationToken = default);
    Task<List<CustomerParcelDto>> GetCustomerParcelsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);
}