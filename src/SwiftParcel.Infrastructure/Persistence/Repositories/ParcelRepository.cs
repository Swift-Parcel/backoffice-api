using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using Domain.Entities;

public class ParcelRepository : IParcelRepository
{
    private readonly AppDbContext _context;

    public ParcelRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Parcel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Parcels
            .Include(p => p.Customer)
            .Include(p => p.Cases)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Parcel?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Parcels
            .Include(p => p.Customer)
            .Include(p => p.Cases)
            .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber, cancellationToken);
    }

    public async Task AddAsync(Parcel parcel, CancellationToken cancellationToken = default)
    {
        await _context.Parcels.AddAsync(parcel, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Parcel parcel, CancellationToken cancellationToken = default)
    {
        _context.Parcels.Update(parcel);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Parcel parcel, CancellationToken cancellationToken = default)
    {
        _context.Parcels.Remove(parcel);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<List<CustomerParcelDto>> GetCustomerParcelsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Parcels
            .Where(p => p.Customer.Email == email)
            .Select(p => new CustomerParcelDto(
                p.TrackingNumber,
                p.Status,
                new CustomerParcelSenderDto(
                    p.Customer.Email,
                    new AddressDto(
                        p.Customer.Address.City,
                        p.Customer.Address.CountryCode,
                        p.Customer.Address.PostalCode,
                        p.Customer.Address.Street,
                        p.Customer.Address.StreetNumber
                    )
                ),
                new CustomerParcelRecipientDto(
                    p.RecipientName,
                    new AddressDto(
                        p.RecipientAddress.City,
                        p.RecipientAddress.CountryCode,
                        p.RecipientAddress.PostalCode,
                        p.RecipientAddress.Street,
                        p.RecipientAddress.StreetNumber
                    )
                ),
                p.CreatedDate,
                p.ServiceType
            ))
            .ToListAsync(cancellationToken);
    }
    
    public async Task<bool> ExistsByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Parcels
            .AnyAsync(p => p.TrackingNumber == trackingNumber, cancellationToken);
    }
    
    public async Task<ParcelStatus?> GetStatusByTrackingNumberAsync(TrackingNumber trackingNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Parcels
            .Where(p => p.TrackingNumber == trackingNumber.Value)
            .Select(p => (ParcelStatus?)p.Status)
            .FirstOrDefaultAsync(cancellationToken);
    }
}