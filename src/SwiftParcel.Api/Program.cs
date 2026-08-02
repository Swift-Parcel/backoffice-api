using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Api.Middleware;
using SwiftParcel.Application;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.Services;
using SwiftParcel.Infrastructure.Persistence;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Infrastructure.Persistence.Seeding;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;
using SwiftParcel.Infrastructure.Services;
using SwiftParcel.Infrastructure.Services.Mock;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExcpetionHandler>();
builder.Services.AddProblemDetails();

//dbContext registration
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

builder.Services.AddScoped<IAppDbContext>(
    sp => sp.GetRequiredService<AppDbContext>());

var legacyConnectionString = builder.Configuration.GetConnectionString("LegacyConnection");
builder.Services.AddDbContext<LegacyDbContext>(options =>
    options.UseNpgsql(legacyConnectionString));

//seeder registration
var seederTypes = typeof(DataSeederOrchestrator).Assembly
    .GetTypes()
    .Where(t => typeof(IEntitySeeder).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

foreach (var type in seederTypes)
{
    builder.Services.AddScoped(typeof(IEntitySeeder), type);
}

builder.Services.AddScoped<DataSeederOrchestrator>();

builder.Services.AddApplication();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();

builder.Services.AddScoped<IParcelService, MockParcelService>();
builder.Services.AddScoped<IDeliveryEstimationService, DeliveryEstimationService>();

builder.Services.AddScoped<ICaseService, MockCaseService>();

builder.Services.AddScoped<ICustomerService, MockCustomerService>();

// Webhook
builder.Services.AddHttpClient<IWebhookClient, WebhookClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["JavaBackend:BaseUrl"]);
});

var app = builder.Build();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var newDbContext = services.GetRequiredService<AppDbContext>();
    await newDbContext.Database.MigrateAsync();

    var migrationService = services.GetRequiredService<DataSeederOrchestrator>();
    await migrationService.RunMigrationIfNeededAsync();
}


app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/", () => "Swift-parcel backoffice is up and running.");
await app.RunAsync();