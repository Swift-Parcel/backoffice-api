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
using SwiftParcel.Application.Common.Models;
using CreateCaseRequest = SwiftParcel.Application.Cases.Commands.CreateCase.CreateCaseRequest;

namespace SwiftParcel.Api.Controllers;

[ApiController]
[Route("api/cases")]
public class CasesController : ApiController
{
    /// <summary>
    /// Retrieves a list of cases scoped to the user's authorized regions.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<CaseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCases(
        [FromQuery] int? customerId,
        [FromQuery] string? customerEmail,
        [FromQuery] string? customerPhone,
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCasesQuery(customerId, customerEmail, customerPhone, searchTerm)
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        return HandleResult(await Mediator.Send(query, cancellationToken));
    }
    
    /// <summary>
    /// Creates a new case with the provided details. Returns the created
    /// case number.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCaseRequest request)
    {
        var command = new CreateCaseCommand(
            request.Title,
            request.Description,
            request.CaseType,
            request.CaseStatus,
            request.CustomerEmail,
            request.HandlerId,
            request.RegionId,
            request.Channel,
            request.TagIds,
            request.ParcelIds,
            request.Priority
        );

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
    
    /// <summary>
    /// Manually assigns a case to a specific handler.
    /// </summary>
    [HttpPost("{caseNumber}/assign")]
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
    /// Changes the status of a case and triggers lifecycle notifications.
    /// </summary>
    [HttpPost("{caseNumber}/change-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeCaseStatus(
        [FromRoute] string caseNumber, 
        [FromBody] ChangeCaseStatusRequest request, 
        CancellationToken cancellationToken)
    {
        var command = new ChangeCaseStatusCommand(caseNumber, request.Status);
        
        var result = await Mediator.Send(command, cancellationToken);
        
        return HandleResult(result);
    }
    
    /// <summary>
    /// Processes a delivery change request outcome and notifies external systems via webhook.
    /// </summary>
    [HttpPost("{caseNumber}/delivery-change/outcome")]
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