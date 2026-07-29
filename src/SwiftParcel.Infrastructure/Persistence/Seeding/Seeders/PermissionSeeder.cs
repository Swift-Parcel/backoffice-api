using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class PermissionSeeder : IEntitySeeder
{
    public int Order => 2;

    public Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}