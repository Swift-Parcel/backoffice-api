using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding;

public class DbSeeder
{
    private readonly AppDbContext _dbContext;

    private readonly LegacyDbContext _oldDbContext;

    private readonly IEnumerable<IEntitySeeder> _seeders;

    private readonly ILogger<DbSeeder> _logger;

    private static async Task ResyncPostgresSequencesAsync(AppDbContext dbContext, CancellationToken cancellationToken)
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

    public DbSeeder(
        AppDbContext dbContext,
        LegacyDbContext oldDbContext,
        IEnumerable<IEntitySeeder> seeders,
        ILogger<DbSeeder> logger)
    {
        _dbContext = dbContext;
        _oldDbContext = oldDbContext;
        _seeders = seeders;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedFromLegacyDbAsync(cancellationToken);

        await SeedSystemUser(cancellationToken);

        await ResyncPostgresSequencesAsync(_dbContext, cancellationToken);
    }

    private async Task SeedFromLegacyDbAsync(CancellationToken cancellationToken = default)
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
                _dbContext.ChangeTracker.Clear();
            }

            await ResyncPostgresSequencesAsync(_dbContext, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("All seeder succeeded, transaction is finished.");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task SeedSystemUser(CancellationToken cancellationToken)
    {
        var systemUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == "system", cancellationToken);
        if (systemUser == null)
        {
            systemUser = new Domain.Entities.User
            {
                Username = "system",
                PasswordHash = string.Empty,
                FullName = "System User",
                RoleId = await _dbContext.Roles
                    .Where(r => r.Name == "Admin")
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync(cancellationToken),
                Email = "system@email.com",
                CreatedDate = DateTime.UtcNow.AddDays(-1)
            };
            await _dbContext.Users.AddAsync(systemUser, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}