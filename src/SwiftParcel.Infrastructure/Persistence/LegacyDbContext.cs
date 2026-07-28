using Microsoft.EntityFrameworkCore;

namespace SwiftParcel.Infrastructure.Persistence;

public class LegacyDbContext:DbContext
{
    public LegacyDbContext(DbContextOptions<LegacyDbContext> options)
        : base(options)
    {
    }
}