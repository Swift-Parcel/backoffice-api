using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Case> Cases { get; }
    DbSet<CaseNote> CaseNotes { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Parcel> Parcels { get; }
    DbSet<Region> Regions { get; }
    DbSet<SlaRule> SlaRules { get; }
    DbSet<Tag> Tags { get; }
    DbSet<User> Users { get; }
    DbSet<Handler> Handlers { get; }
    DbSet<Role> Roles { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}