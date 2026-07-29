using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding;

public class DataSeederOrchestrator
{
    private readonly AppDbContext _dbContext;
    private readonly LegacyDbContext _oldDbContext;
    private readonly IEnumerable<IEntitySeeder> _seeders;
    private readonly ILogger<DataSeederOrchestrator> _logger;

    public DataSeederOrchestrator(
        AppDbContext dbContext,
        LegacyDbContext oldDbContext,
        IEnumerable<IEntitySeeder> seeders,
        ILogger<DataSeederOrchestrator> logger)
    {
        _dbContext = dbContext;
        _oldDbContext = oldDbContext;
        _seeders = seeders;
        _logger = logger;
    }

    public async Task RunMigrationIfNeededAsync(CancellationToken cancellationToken = default)
    {
        if (await _dbContext.Parcels.AnyAsync(cancellationToken)) return;
        
        var orderedSeeders = _seeders.OrderBy(s => s.Order).ToList();

        if (!orderedSeeders.Any())
        {
            _logger.LogInformation("There are no IEntitySeeder implementations available.");
            return;
        }

        _logger.LogInformation("{Count} seeder have been started...", orderedSeeders.Count);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var seeder in orderedSeeders)
            {
                var seederName = seeder.GetType().Name;
                _logger.LogInformation("[{Order}] Running seeder: {SeederName}...", seeder.Order, seederName);

                await seeder.SeedAsync(_oldDbContext,_dbContext, cancellationToken);

                _logger.LogInformation("[{Order}] Seed succeeded: {SeederName}", seeder.Order, seederName);
            }

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("All seeder succeeded, transaction is finished.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}