using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Domain.Exceptions;

namespace SwiftParcel.Api.Middleware;

public class GlobalExcpetionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExcpetionHandler> _logger;

    public GlobalExcpetionHandler(ILogger<GlobalExcpetionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, 
        Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, code) = exception switch
        {
            DomainException domainEx => (
                (int)domainEx.StatusCode, 
                domainEx.StatusCode.ToString(),
                domainEx.Code
            ),
            _ => (
                StatusCodes.Status500InternalServerError, 
                "InternalServerError", 
                "internal_server_error"
            )
        };

        if (statusCode >= 500)
        {
            _logger.LogError(exception, "An unhandled exception occurred:{Message}",
                exception.Message);
        }
        else
        {
            _logger.LogWarning("Domain exception occurred: {Code} - {Message}",
                code, exception.Message);
        }
        
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["code"] = code
            }
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}