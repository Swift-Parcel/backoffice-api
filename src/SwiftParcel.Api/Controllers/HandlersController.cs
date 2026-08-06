using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.DTO.Handlers;
using SwiftParcel.Application.Handlers.Commands.CreateHandler;
using SwiftParcel.Application.Handlers.Commands.UpdateHandler;
using SwiftParcel.Application.Handlers.Commands.UpdateHandlerStatus;
using SwiftParcel.Application.Handlers.Queries.GetCurrentHandler;
using SwiftParcel.Application.Handlers.Queries.GetHandlerById;
using SwiftParcel.Application.Handlers.Queries.GetHandlers;

namespace SwiftParcel.Api.Controllers;

[ApiController]
[Route("api/handlers")]
[Authorize(Roles = "Supervisor,Admin")]
public class HandlersController : ApiController
{
    /// <summary>
    /// Gets the current user's handler profile
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(HandlerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentHandler(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetCurrentHandlerQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Lists handlers
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Supervisor,Admin")]
    [ProducesResponseType(typeof(List<HandlerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHandlers(
        [FromQuery] bool? isActive, 
        [FromQuery] string? department, 
        CancellationToken cancellationToken)
    {
        var query = new GetHandlersQuery(isActive, department);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Gets a specific handler by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Supervisor,Admin")]
    [ProducesResponseType(typeof(HandlerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHandlerById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetHandlerByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }
    
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