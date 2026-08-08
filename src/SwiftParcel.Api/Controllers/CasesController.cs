using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Cases.Commands.AssignCase;
using SwiftParcel.Application.Cases.Commands.ChangeCaseStatusCommand;
using SwiftParcel.Application.Cases.Commands.CreateCase;
using SwiftParcel.Application.Cases.Commands.DeliveryChangeCommand;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Application.Cases.Commands.UpdateCaseStatusCommand;
using SwiftParcel.Application.Cases.Queries.GetCases;

namespace SwiftParcel.Api.Controllers;

[ApiController]
[Route("api/cases")]
[Authorize]
public class CasesController : ApiController
{
    /// <summary>
    /// Retrieves a list of cases scoped to the user's authorized regions.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Operator,Supervisor,Admin")]
    [ProducesResponseType(typeof(List<CaseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCases(
        [FromQuery] int? customerId,
        [FromQuery] string? customerEmail,
        [FromQuery] string? customerPhone,
        CancellationToken cancellationToken)
    {
        var query = new GetCasesQuery(customerId, customerEmail, customerPhone);
        return HandleResult(await Mediator.Send(query, cancellationToken));
    }
    
    /// <summary>
    /// Creates a new case with the provided details. Returns the created
    /// case number.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Operator,Supervisor,Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCaseCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
    
    /// <summary>
    /// Manually assigns a case to a specific handler.
    /// </summary>
    [HttpPost("{caseNumber}/assign")]
    [Authorize(Roles = "Operator,Supervisor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignCase(
        [FromRoute] string caseNumber, 
        [FromBody] AssignCaseCommand command)
    {
        var result = await Mediator.Send(command with { CaseNumber = caseNumber });
        
        return HandleResult(result);
    }

    /// <summary>
    /// Changes the status of a case and triggers lifecycle notifications.
    /// </summary>
    [HttpPost("{caseNumber}/change-status")]
    [Authorize(Roles = "Operator,Supervisor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeCaseStatus(string caseNumber, [FromBody] ChangeStatusCommand command, CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(command with { CaseNumber = caseNumber }, cancellationToken);
        return HandleResult(result);
    }
    
    /// <summary>
    /// Processes a delivery change request outcome and notifies external systems via webhook.
    /// </summary>
    [HttpPost("{caseNumber}/delivery-change/outcome")]
    [Authorize(Roles = "Operator,Supervisor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProcessDeliveryChange(
        [FromRoute] string caseNumber, 
        [FromBody] DeliveryChangeOutcome outcome, 
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new ProcessDeliveryChangeCommand(caseNumber, outcome), cancellationToken);
        return HandleResult(result);
    }
}