using System.Text.Json.Serialization;
using SwiftParcel.Api.Extensions;
using SwiftParcel.Api.Middleware;
using SwiftParcel.Application;
using SwiftParcel.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

Action<System.Text.Json.JsonSerializerOptions> configureJsonOptions = options =>
{
    options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
    
    options.Converters.Add(new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.SnakeCaseUpper));
};

builder.Services.AddControllers()
    .AddJsonOptions(options => configureJsonOptions(options.JsonSerializerOptions));

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => 
    configureJsonOptions(options.SerializerOptions));

builder.Services.AddApplication(builder.Configuration)
                .AddInfrastructure(builder.Configuration);

builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

await app.SeedDatabaseAsync();

app.UseExceptionHandler(options =>
{
    options.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new { message = "An unexpected error occurred." });
    });
});
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();