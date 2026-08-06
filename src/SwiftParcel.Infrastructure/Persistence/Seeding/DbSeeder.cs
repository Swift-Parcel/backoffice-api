using Microsoft.EntityFrameworkCore;
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
        var tables = new[] { "users", "roles", "customers", "cases", "case_notes", "parcels", "handlers" };

        const string sql = @"
        DO $$
        DECLARE
            tbl text;
            seq text;
            max_id bigint;
        BEGIN
            FOREACH tbl IN ARRAY @p0
            LOOP
                seq := pg_get_serial_sequence(quote_ident(tbl), 'id');
                
                IF seq IS NOT NULL THEN
                    EXECUTE format('SELECT COALESCE(MAX(id), 0) FROM %I', tbl) INTO max_id;
                    
                    IF max_id = 0 THEN
                        PERFORM setval(seq, 1, false);
                    ELSE
                        PERFORM setval(seq, max_id, true);
                    END IF;
                END IF;
            END LOOP;
        END $$;";

        await dbContext.Database.ExecuteSqlRawAsync(sql, new object[] { tables }, cancellationToken);
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
                Email = "system@email.com"
            };
            await _dbContext.Users.AddAsync(systemUser, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}