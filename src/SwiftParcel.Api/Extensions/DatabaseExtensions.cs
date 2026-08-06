using Microsoft.EntityFrameworkCore;
using SwiftParcel.Infrastructure.Persistence;
using SwiftParcel.Infrastructure.Persistence.Seeding;

namespace SwiftParcel.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var newDbContext = services.GetRequiredService<AppDbContext>();
        await newDbContext.Database.MigrateAsync();

        var migrationService = services.GetRequiredService<DbSeeder>();
        await migrationService.SeedAsync();
        
        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
        {
            var testDataSeeder = services.GetRequiredService<TestDataSeeder>();
            await testDataSeeder.SeedAsync();
        }
    }
}