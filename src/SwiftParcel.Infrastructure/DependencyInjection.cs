using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Authentication;
using SwiftParcel.Application.Common.Settings;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.Services;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Infrastructure.Authentication;
using SwiftParcel.Infrastructure.Persistence;
using SwiftParcel.Infrastructure.Persistence.Seeding;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;
using SwiftParcel.Infrastructure.Services;
using PasswordHasher = SwiftParcel.Infrastructure.Authentication.PasswordHasher;

namespace SwiftParcel.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
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
                    npgsqlOptions.MapEnum<AuditAction>("enum_action");
                    npgsqlOptions.MapEnum<EntityType>("enum_entity_type");
                })
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        var legacyConnectionString = configuration.GetConnectionString("LegacyConnection");
        services.AddDbContext<LegacyDbContext>(options =>
            options.UseNpgsql(legacyConnectionString));
        
        var seederTypes = typeof(DataSeederOrchestrator).Assembly
            .GetTypes()
            .Where(t => typeof(IEntitySeeder).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in seederTypes)
        {
            services.AddScoped(typeof(IEntitySeeder), type);
        }

        services.AddScoped<DataSeederOrchestrator>();
        
        services.AddScoped<IDeliveryEstimationService, DeliveryEstimationService>();

        services.AddHttpClient<IWebhookClient, WebhookClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["JavaBackend:BaseUrl"] ?? string.Empty);
        });
        
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret))
            });

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        services.Configure<ApiKeySettings>(
            configuration.GetSection(ApiKeySettings.SectionName));
        
        return services;
    }
}