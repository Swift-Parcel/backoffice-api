using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Case> Cases { get; }
    DbSet<CaseNote> CaseNotes { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Parcel> Parcels { get; }
    DbSet<SlaRule> SlaRules { get; }
    DbSet<Region> Regions { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}