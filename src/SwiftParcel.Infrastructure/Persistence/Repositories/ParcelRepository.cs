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
}