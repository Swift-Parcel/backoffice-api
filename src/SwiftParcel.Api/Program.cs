using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SwiftParcel.Api.Middleware;
using SwiftParcel.Application;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.Services;
using SwiftParcel.Infrastructure.Persistence;
using SwiftParcel.Infrastructure;
using SwiftParcel.Infrastructure.Persistence.Seeding;
using SwiftParcel.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddApplication();

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "SwiftParcel Back-Office API",
            Version = "v1",
            Description = @"
### 🔐 Development Credentials:
* **Admin Login:** `admin` / `admin`

### 🤝 Java Portal Integration:
* **Header:** `X-Api-Key`
* **Secret Value:** `SwiftParcel_Java_Integration_Shared_Secret_2026!`
"
        };
        return Task.CompletedTask;
    });
});

builder.Services.AddScoped<IDeliveryEstimationService, DeliveryEstimationService>();

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

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();