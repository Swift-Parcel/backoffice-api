using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Cases.Commands.AssignCase;
using SwiftParcel.Application.Cases.Commands.CreateCase;
using SwiftParcel.Application.Cases.Commands.ProcessDeliveryChange;
using SwiftParcel.Application.Cases.Queries.GetCaseNotes;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Application.Cases.Commands.UpdateCaseStatus;
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
    public async Task<IActionResult> GetCases(CancellationToken cancellationToken)
    {
        var query = new GetCasesQuery();

        return HandleResult(await Mediator.Send(query, cancellationToken));
    }
    
    /// <summary>
    /// Allows an agent to get all notes for a specific case.
    /// </summary>
    [HttpGet("{caseNumber}/notes")]
    [Authorize(Roles = "Read-Only,Operator,Supervisor,Admin")]
    [ProducesResponseType(typeof(IReadOnlyList<CaseNoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerCaseNotes([FromRoute] string caseNumber)
    {
        var query = new GetCaseNotesQuery(caseNumber);
        
        return HandleResult(await Mediator.Send(query));
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