using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class SystemConfigSeeder : IEntitySeeder
{
    public int Order => 20;
    public Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}