using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SwiftParcel.Api.Middleware;

public class GlobalExcpetionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExcpetionHandler> _logger;

    public GlobalExcpetionHandler(ILogger<GlobalExcpetionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred:{Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server error",
            Detail = "An unexpected error occurred processing your request."
        };
        
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}