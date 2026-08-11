using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using SwiftParcel.Application.Exceptions;
using SwiftParcel.Domain.Exceptions;

namespace SwiftParcel.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        var (statusCode, message) = exception switch
        {
            ValidationException valEx => (
                StatusCodes.Status400BadRequest, 
                valEx.Message
            ),
            UnauthorizedException unauthEx => (
                StatusCodes.Status401Unauthorized, 
                string.IsNullOrWhiteSpace(unauthEx.Message) ? "Unauthorized" : unauthEx.Message
            ),
            ForbiddenException forbiddenEx => (
                StatusCodes.Status403Forbidden, 
                string.IsNullOrWhiteSpace(forbiddenEx.Message) ? "Forbidden" : forbiddenEx.Message
            ),
            DomainException domainEx => (
                (int)domainEx.StatusCode, 
                domainEx.Message
            ),
            _ => (
                StatusCodes.Status500InternalServerError, 
                "An unexpected error occurred."
            )
        };

        if (statusCode >= 500)
        {
            _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Handled exception occurred ({StatusCode}): {Message}", statusCode, exception.Message);
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new { message }, cancellationToken);

        return true;
    }
}