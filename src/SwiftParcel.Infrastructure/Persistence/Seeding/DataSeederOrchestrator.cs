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

    public async Task RunAsync()
    {
        Console.WriteLine("Starting data seeding...");
    }
}