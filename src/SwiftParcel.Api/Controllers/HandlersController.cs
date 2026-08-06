using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.DTO.Handlers;
using SwiftParcel.Application.Handlers.Commands.CreateHandler;
using SwiftParcel.Application.Handlers.Commands.UpdateHandler;
using SwiftParcel.Application.Handlers.Commands.UpdateHandlerStatus;

namespace SwiftParcel.Api.Controllers;

[ApiController]
[Route("api/handlers")]
[Authorize(Roles = "Supervisor,Admin")]
public class HandlersController : ApiController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateHandler([FromBody] CreateHandlerCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHandler([FromRoute] int id, [FromBody] UpdateHandlerRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateHandlerCommand(id, request.Department, request.MaxCases);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
    
    [HttpPatch("{id:int}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActivateHandler([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new UpdateHandlerStatusCommand(id, true), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:int}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeactivateHandler([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new UpdateHandlerStatusCommand(id, false), cancellationToken);
        return HandleResult(result);
    }
}