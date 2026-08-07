using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Cases.Commands.AddCaseNote;
using SwiftParcel.Application.Cases.Queries.GetCustomerCaseNotes;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Infrastructure.Authentication;

namespace SwiftParcel.Api.Controllers.Integration;

[ApiController]
[ApiKeyAuth]
[Route("api/integration/cases/{caseNumber}/notes")]
public class IntegrationCaseNotesController : ApiController
{
    /// <summary>
    /// Get all customer-visible notes for a specific case.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CaseNoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerCaseNotes([FromRoute] string caseNumber)
    {
        var query = new GetCustomerCaseNotesQuery(caseNumber);
        
        return HandleResult(await Mediator.Send(query));
    }
    
    /// <summary>
    /// Accepts a customer-visible note/message submitted from the Customer Portal (Java Team).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCustomerNote(
        [FromRoute] string caseNumber, 
        [FromBody] AddCustomerNoteRequest request)
    {
        var command = new AddCustomerNoteCommand(
            caseNumber, 
            request.Message, 
            request.CustomerEmail, 
            request.Attachment);
            
        return HandleResult(await Mediator.Send(command));
    }
}