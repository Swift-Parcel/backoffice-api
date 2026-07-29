namespace SwiftParcel.Infrastructure.Persistence.Seeding;

public class DataSeederOrchestrator
{
    private readonly LegacyDbContext _oldDb;
    private readonly AppDbContext _newDb;
    
    public DataSeederOrchestrator(LegacyDbContext oldDb, AppDbContext newDb)
    {
        _oldDb = oldDb;
        _newDb = newDb;
    }

    public async Task RunMigrationIfNeededAsync()
    {
        Console.WriteLine($"Starting data seeding...{_oldDb.Database}, {_newDb.Addresses}");
    }
}