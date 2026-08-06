using Microsoft.OpenApi;

namespace SwiftParcel.Api.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "SwiftParcel Back-Office API",
                    Version = "v1",
                    Description = @"
### Development Credentials:
* **Admin Login:** `admin` / `admin`

### Java Portal Integration:
* **Header:** `X-Api-Key`
* **Secret Value:** `SwiftParcel_Java_Integration_Shared_Secret_2026!`
"
                };
                return Task.CompletedTask;
            });
        });

        return services;
    }
}