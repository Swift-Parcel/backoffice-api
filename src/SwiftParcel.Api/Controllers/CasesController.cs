using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Cases.Commands.CreateCase;
using SwiftParcel.Application.Cases.Queries.GetCaseNotes;
using SwiftParcel.Application.DTO.Cases;

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
    /// Allows a handler or operator to add a timestamped note (internal or customer-visible) to a case.
    /// </summary>
    [HttpPost("{caseNumber}/notes")]
    [Authorize(Roles = "Operator,Supervisor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddHandlerNote(
        [FromRoute] string caseNumber, 
        [FromBody] AddHandlerNoteRequest request)
    {
        int handlerId = 1; 

        var command = new AddHandlerNoteCommand(
            caseNumber, 
            request.Message, 
            request.IsInternal, 
            handlerId, 
            request.Attachment
        );

        var result = await Mediator.Send(command);

        return HandleResult(result);
    }
    
    /// <summary>
    /// Get all notes for a specific case.
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
}