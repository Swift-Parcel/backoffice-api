using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Api.Controllers.Integration;

[ApiController]
[Route("api/integration/cases")]
public class IntegrationCasesController : ApiController
{
    /// <summary>
    /// Accepts a customer-visible note/message submitted from the Customer Portal (Java Team).
    /// </summary>
    [HttpPost("{caseNumber}/notes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCustomerNote(
        [FromRoute] string caseNumber, 
        [FromBody] AddCustomerNoteRequest request)
    {
        var command = new AddCustomerNoteCommand(
            caseNumber, 
            request.Message, 
            request.CustomerEmail, 
            request.Attachment
        );

        var result = await Mediator.Send(command);
        
        return HandleResult(result);
    }
}