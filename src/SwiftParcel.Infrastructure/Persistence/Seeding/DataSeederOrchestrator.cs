using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding;

public class DataSeederOrchestrator
{
    private readonly AppDbContext _dbContext;

    private readonly LegacyDbContext _oldDbContext;

    private readonly IEnumerable<IEntitySeeder> _seeders;

    private readonly ILogger<DataSeederOrchestrator> _logger;

    private async Task ResyncPostgresSequencesAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var entityTypes = dbContext.Model.GetEntityTypes();

        foreach (var entityType in entityTypes)
        {
            var tableName = entityType.GetTableName();
            if (string.IsNullOrEmpty(tableName)) 
                continue;

            var pk = entityType.FindPrimaryKey();
            if (pk == null || pk.Properties.Count != 1) 
                continue;

            var pkProperty = pk.Properties[0];
            var storeObject = StoreObjectIdentifier.Table(tableName, entityType.GetSchema());
            var columnName = pkProperty.GetColumnName(storeObject);

            var sql = $@"
        DO $$
        DECLARE
            seq TEXT;
        BEGIN
            seq := pg_get_serial_sequence('""{tableName}""', '{columnName}');
            
            IF seq IS NOT NULL THEN
                EXECUTE format('SELECT setval(''%s'', COALESCE((SELECT MAX(""{columnName}"") FROM ""{tableName}""), 1))', seq);
            END IF;
        END $$;";
            
            await dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }
    }
    
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

                await seeder.SeedAsync(_oldDbContext, _dbContext, cancellationToken);
                
                _logger.LogInformation("[{Order}] Seed succeeded: {SeederName}", seeder.Order, seederName);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            
            await ResyncPostgresSequencesAsync(_dbContext, cancellationToken);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("All seeder succeeded, transaction is finished.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error has occured while seeding. Transaction is rolled back.");
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}