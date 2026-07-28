using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class ParcelSeeder : IEntitySeeder
{
    public int Order => 6;

    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var legacyParcels = dbContext.Database.SqlQueryRaw<LegacyParcelDto>("SELECT [all the columns that we need]")
            .ToList();

        var newParcels = new List<Parcel>();

        foreach (var oldParcel in legacyParcels)
        {
            var newParcel = new Parcel
            {
                //mapping of the required fields
            };
        }

        await dbContext.Parcels.AddRangeAsync(newParcels, cancellationToken);
    }


    private record LegacyParcelDto(
        string id,
        string trackingNumber,
        string recName,
        string recAddress,
        string weight,
        string dimensions,
        string status,
        string createdDate,
        string deliveredDate,
        string serviceType,
        string declaredValue,
        string customerId);
}