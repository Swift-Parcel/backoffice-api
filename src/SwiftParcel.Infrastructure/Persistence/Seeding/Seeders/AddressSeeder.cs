using Microsoft.EntityFrameworkCore;
using SwiftParcel.Infrastructure.Parsers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class AddressSeeder : IEntitySeeder
{
    public int Order => 60;
    
    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Addresses.AnyAsync(cancellationToken))
        {
            return;
        }
        
        var legacyAddresses = await oldDbContext.Database
            .SqlQueryRaw<LegacyAddressDto>(@"SELECT sender_address AS address FROM parcels UNION SELECT recipient_address FROM parcels UNION SELECT address FROM customers")
            .ToListAsync(cancellationToken);
        
        var newAddresses = legacyAddresses.Select(legacyAddress => AddressParserHelper.SplitStringAddress(legacyAddress.address)).ToList();

        await dbContext.Addresses.AddRangeAsync(newAddresses, cancellationToken);
    }
    
    private record LegacyAddressDto(
        string address);
}