using MediatR;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Domain.Shared;

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
            ErrorType.NotFound => NotFound(new { message = error.Message }),
            ErrorType.Validation => BadRequest(new { message = error.Message }),
            ErrorType.Conflict => Conflict(new { message = error.Message }),
            ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new { message = error.Message }),
            _ => BadRequest(new { message = error?.Message ?? "An unexpected error occurred." })
        };
    }
}