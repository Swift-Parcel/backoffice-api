namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

using Microsoft.EntityFrameworkCore;
using Interfaces;
using Helpers;

public class CaseSeeder : IEntitySeeder
{
    public int Order => 7;

    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        // Build an in-memory dictionary mapping tracking numbers to Parcel entities
        // Including the Parcels directly so EF Core can manage the relationship
        var parcelMap = await dbContext.Parcels
            .ToDictionaryAsync(p => p.TrackingNumber, p => p, cancellationToken);

        // Fetch existing Cases with their loaded Parcels to prevent duplicate assignments
        var cases = await dbContext.Cases
            .Include(c => c.Parcels)
            .ToDictionaryAsync(c => c.Id, cancellationToken);

        // Raw SQL query to fetch CSV-formatted tracking numbers from legacy cases table
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT id, parcel_tracking_numbers FROM cases WHERE parcel_tracking_numbers IS NOT NULL AND parcel_tracking_numbers != ''";

        await dbContext.Database.OpenConnectionAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var hasChanges = false;

        // Process results row-by-row from the database reader stream
        while (await reader.ReadAsync(cancellationToken))
        {
            var caseId = reader.GetInt32(0);
            var rawTrackingNumbers = reader.GetString(1);

            // Parse and clean CSV tracking numbers
            var trackingNumbers = StringParserHelper.ParseCsvString(rawTrackingNumbers);

            if (cases.TryGetValue(caseId, out var currentCase))
            {
                foreach (var tracking in trackingNumbers)
                {
                    // Match tracking number and ensure it is not already linked to the Case
                    if (parcelMap.TryGetValue(tracking, out var parcel))
                    {
                        if (!currentCase.Parcels.Any(p => p.Id == parcel.Id))
                        {
                            currentCase.Parcels.Add(parcel);
                            hasChanges = true;
                        }
                    }
                }
            }
        }

        // Save changes if any new relationships were established
        if (hasChanges)
            await dbContext.SaveChangesAsync(cancellationToken);
    }
}