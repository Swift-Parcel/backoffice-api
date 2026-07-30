using Microsoft.EntityFrameworkCore;
using SwiftParcel.Api;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.Services;
using SwiftParcel.Infrastructure.Persistence; 
using SwiftParcel.Domain.Enums;
using SwiftParcel.Infrastructure.Persistence.Seeding;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
        options
            .UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MapEnum<ParcelStatus>("enum_parcel_status");
                npgsqlOptions.MapEnum<Timeslot>("enum_timeslot");
                npgsqlOptions.MapEnum<ServiceType>("enum_service_type");
                npgsqlOptions.MapEnum<CaseType>("enum_case_type");
                npgsqlOptions.MapEnum<CaseStatus>("enum_case_status");
                npgsqlOptions.MapEnum<Priority>("enum_priority");
                npgsqlOptions.MapEnum<Channel>("enum_channel");
                npgsqlOptions.MapEnum<DayOfWeek>("enum_day_of_week");
                npgsqlOptions.MapEnum<SwiftParcel.Domain.Enums.AuditAction>("enum_action");
                npgsqlOptions.MapEnum<EntityType>("enum_entity_type");
            })
            .UseSnakeCaseNamingConvention()
);

var legacyConnectionString = builder.Configuration.GetConnectionString("LegacyConnection");
builder.Services.AddDbContext<LegacyDbContext>(options =>
    options.UseNpgsql(legacyConnectionString));

builder.Services.AddScoped<DataSeederOrchestrator>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddScoped<IParcelIntegrationService, MockParcelService>();
builder.Services.AddScoped<IDeliveryEstimationService, DeliveryEstimationService>();

var app = builder.Build();

/*  --- TEMPORARILY COMMENTED OUT UNTIL DATABASE IS READY ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var newDbContext = services.GetRequiredService<AppDbContext>();
        await newDbContext.Database.MigrateAsync();

        var migrationService = services.GetRequiredService<DataSeederOrchestrator>();
        await migrationService.RunMigrationIfNeededAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error occured while migrating the database.");
        throw;
    }
}
*/

app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/", () => "Swift-parcel backoffice is up and running.");
await app.RunAsync();