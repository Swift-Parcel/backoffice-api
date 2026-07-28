using Microsoft.EntityFrameworkCore;
using SwiftParcel.Infrastructure.Persistence; 
using SwiftParcel.Domain.Enums;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SwiftParcel");

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

var legacyConnectionString = builder.Configuration.GetConnectionString("Legacy");
builder.Services.AddDbContext<LegacyDbContext>(options =>
    options.UseNpgsql(legacyConnectionString));

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();