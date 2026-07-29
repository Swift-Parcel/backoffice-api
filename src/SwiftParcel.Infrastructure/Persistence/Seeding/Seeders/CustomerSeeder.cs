using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class CustomerSeeder : IEntitySeeder
{
    public int Order => 6;

    public Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}