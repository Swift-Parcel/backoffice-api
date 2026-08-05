using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Cases.Commands.AssignCase;
using SwiftParcel.Application.Cases.Commands.CreateCase;
using SwiftParcel.Application.Cases.Commands.ProcessDeliveryChange;
using SwiftParcel.Application.Cases.Queries.GetCaseNotes;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Application.Cases.Commands.UpdateCaseStatus;

namespace SwiftParcel.Api.Controllers;

[ApiController]
[Route("api/cases")]
[Authorize]
public class CasesController : ApiController
{
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
    /// Manually assigns a case to a specific handler. Enforces handler capacity limits.
    /// </summary>
    [HttpPost("{caseNumber}/assign")]
    [Authorize(Roles = "Operator,Supervisor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignCase(
        [FromRoute] string caseNumber, 
        [FromBody] AssignCaseRequest request)
    {
        var command = new AssignCaseCommand(caseNumber, request.HandlerId);
        
        var result = await Mediator.Send(command);
        
        return HandleResult(result);
    }

    /// <summary>
    /// Updates the status of a case and triggers lifecycle notifications.
    /// </summary>
    [HttpPut("{caseNumber}/status")]
    [Authorize(Roles = "Operator,Supervisor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCaseStatus(string caseNumber, [FromBody] CaseStatus newStatus, CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new UpdateCaseStatusCommand(caseNumber, newStatus), cancellationToken);
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