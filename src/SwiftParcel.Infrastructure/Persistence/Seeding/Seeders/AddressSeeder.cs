using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class AddressSeeder : IEntitySeeder
{
    public int Order => 60;
    
    
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Addresses.AnyAsync(cancellationToken))
        {
            return;
        }
        
        
        var newAddresses = new List<Address>();
        
        
        await dbContext.Addresses.AddRangeAsync(newAddresses, cancellationToken);
    }
    
    private record LegacyAddressDto(
        string Address);
}