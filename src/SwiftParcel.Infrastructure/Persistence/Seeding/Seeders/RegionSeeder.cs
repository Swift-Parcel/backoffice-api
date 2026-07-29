using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class RegionSeeder : IEntitySeeder
{
    public int Order => 4;

    public Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}