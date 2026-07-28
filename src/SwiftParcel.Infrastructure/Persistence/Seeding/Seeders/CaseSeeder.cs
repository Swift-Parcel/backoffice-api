namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

using Microsoft.EntityFrameworkCore;
using Interfaces;
using Domain.Entities;
using Helpers;

public class CaseSeeder : IEntitySeeder
{
    public int Order => 7;

    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        // Build an in-memory lookup dictionary from existing parcels (O(1) lookup time)
        var parcelMap = await dbContext.Parcels
            .ToDictionaryAsync(p => p.TrackingNumber, p => p.Id, cancellationToken);

        // Load already existing relations into a HashSet to prevent duplicate insertions
        var existingCaseParcels = await dbContext.CaseParcels
            .Select(cp => new { cp.CaseId, cp.ParcelId })
            .ToHashSetAsync(cancellationToken);

        var caseParcelsToInsert = new List<CaseParcel>();

        // Raw SQL query to fetch CSV-formatted tracking numbers from legacy table
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT id, parcel_tracking_numbers FROM cases WHERE parcel_tracking_numbers IS NOT NULL AND parcel_tracking_numbers != ''";
        
        await dbContext.Database.OpenConnectionAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        // Process results row-by-row from the database reader stream
        while (await reader.ReadAsync(cancellationToken))
        {
            var caseId = reader.GetInt32(0);
            var rawTrackingNumbers = reader.GetString(1);

            // Parse and clean comma-separated values
            var trackingNumbers = StringParserHelper.ParseCsvString(rawTrackingNumbers);

            foreach (var tracking in trackingNumbers)
            {
                // Match tracking number against the new Parcels table ID
                if (parcelMap.TryGetValue(tracking, out var parcelId))
                {
                    var pair = new { CaseId = caseId, ParcelId = parcelId };

                    // Ensure record doesn't exist in the database or in the current batch
                    if (!existingCaseParcels.Contains(pair) &&
                        !caseParcelsToInsert.Any(cp => cp.CaseId == caseId && cp.ParcelId == parcelId))
                    {
                        caseParcelsToInsert.Add(new CaseParcel
                        {
                            CaseId = caseId,
                            ParcelId = parcelId
                        });
                    }
                }
            }
        }

        // Bulk insert mapped entities in a single transaction
        if (caseParcelsToInsert.Any())
        {
            await dbContext.CaseParcels.AddRangeAsync(caseParcelsToInsert, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}