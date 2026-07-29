using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class SlaRuleSeeder : IEntitySeeder
{
    public int Order => 13;

    public Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}