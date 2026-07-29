namespace SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

public interface IEntitySeeder
{
    int Order { get; }
    
    Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default);
}