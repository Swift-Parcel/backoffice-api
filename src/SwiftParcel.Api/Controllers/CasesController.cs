using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Cases.Commands.CreateCase;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Api.Controllers;

public class CasesController : ApiController
{
    [HttpPost]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddHandlerNote(
        [FromRoute] string caseNumber, 
        [FromBody] AddHandlerNoteRequest request)
    {
        // TODO: In Phase 2/Auth stage, extract this from HttpContext.User claims:
        // var handlerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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
}