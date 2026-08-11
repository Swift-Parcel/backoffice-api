using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Common.Models;
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
public class HandlersController : ApiController
{
    /// <summary>
    /// Gets the current user's handler profile
    /// </summary>
    [HttpGet("me")]
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
    [ProducesResponseType(typeof(PagedList<HandlerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHandlers(
        [FromQuery] bool? isActive,
        [FromQuery] string? department,
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetHandlersQuery(isActive, department, searchTerm)
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Gets a specific handler by ID
    /// </summary>
    [HttpGet("{id:int}")]
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
    public async Task<IActionResult> CreateHandler([FromBody] CreateHandlerCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHandler([FromRoute] int id, [FromBody] UpdateHandlerCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send( command with {Id = id}, cancellationToken);
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