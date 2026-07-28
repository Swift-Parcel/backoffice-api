using Microsoft.EntityFrameworkCore;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class ParcelSeeder : IEntitySeeder
{
    public int Order => 6;
    public static int Id { get; } = 0;

    public Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var legacyParcels = dbContext.Database.SqlQueryRaw<LegacyParcelDto>("SELECT")
    }
    
    
    private record LegacyParcelDto(string id, string trackingNumber, string recName, string recAddress,
        string weight, string dimensions, string status, string createdDate, string deliveredDate,
        string serviceType, string declaredValue, string customerId)
}
