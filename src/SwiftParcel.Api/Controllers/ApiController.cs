using MediatR;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Api.Controllers;

[ApiController]
[Route("api/integration/[controller]")]
public class ApiController : ControllerBase
{
    private ISender? _mediator;

    //lazy-loaded MediatR Sender
    protected ISender Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value is null ? NoContent() : Ok(result.Value);
        }

        return result.Error?.Type switch
        {
            ErrorType.NotFound => NotFound(new
            {
                message = $"message:{result.Error.Description}"
            }),

            ErrorType.Validation => BadRequest(new
            {
                message = $"message:{result.Error.Description}"
            }),
            
            ErrorType.Conflict => Conflict(new
            {
                message = $"message:{result.Error.Description}"
            }),
            
            _ => BadRequest(new
            {
                message = $"message::{result.Error?.Description}"
            })
        };
    }
}