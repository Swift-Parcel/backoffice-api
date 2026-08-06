using MediatR;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Api.Controllers;

[ApiController]
[Route("api/integration/[controller]")]
public class ApiController : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return HandleFailure(result.Error);
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value is null ? NoContent() : Ok(result.Value);
        }

        return HandleFailure(result.Error);
    }

    private IActionResult HandleFailure(Error? error)
    {
        return error?.Type switch
        {
            ErrorType.NotFound => NotFound(new { message = error.Description }),
            ErrorType.Validation => BadRequest(new { message = error.Description }),
            ErrorType.Conflict => Conflict(new { message = error.Description }),
            _ => BadRequest(new { message = error?.Description ?? "An unexpected error occurred." })
        };
    }
}