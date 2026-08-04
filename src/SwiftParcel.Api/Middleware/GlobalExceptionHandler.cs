using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, 
        Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ValidationException valEx)
        {
            _logger.LogWarning("Validation failed: {Message}", valEx.Message);
            
            var validationProblemDetails = new HttpValidationProblemDetails(valEx.Errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = valEx.Message,
                Instance = httpContext.Request.Path,
                Extensions = 
                {
                    ["code"] = "validation_error"
                }
            };
            
            httpContext.Response.StatusCode = validationProblemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);
            return true;
        }
        
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
            Detail = exception.InnerException?.Message ?? exception.Message, 
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