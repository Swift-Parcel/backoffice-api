using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SwiftParcel.Application.Common.Settings;

namespace SwiftParcel.Infrastructure.Authentication;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var apiKeySettings = context.HttpContext.RequestServices
            .GetRequiredService<IOptions<ApiKeySettings>>().Value;
        
        if (!context.HttpContext.Request.Headers.TryGetValue(
                apiKeySettings.HeaderName, out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                message = $"Missing header {apiKeySettings.HeaderName}" +
                          $" It is needed for Java integration calls"
            });
            return;
        }
        
        if(!string.Equals(extractedApiKey, apiKeySettings.SecretKey
               , StringComparison.Ordinal))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                message = "Invalid API Key"
            });
            return;
        }

        await next();
    }
}