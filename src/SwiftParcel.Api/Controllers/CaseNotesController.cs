using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Cases.Commands.AddCaseNote;
using SwiftParcel.Application.Cases.Queries.GetCaseNotes;
using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Api.Controllers;

[ApiController]
[Route("api/cases/{caseNumber}/notes")]
public class CaseNotesController : ApiController
{
    /// <summary>
    /// Allows an agent to get all notes for a specific case.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CaseNoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNotes(
        [FromRoute] string caseNumber,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCaseNotesQuery(caseNumber)
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        return HandleResult(await Mediator.Send(query));
    }

    /// <summary>
    /// Allows a handler or operator to add a timestamped note (internal or customer-visible) to a case.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddHandlerNote(
        [FromRoute] string caseNumber,
        [FromBody] AddHandlerNoteRequest request)
    {
        var command = new AddHandlerNoteCommand(
            caseNumber,
            request.Message,
            request.IsInternal,
            request.Attachment
        );

        var result = await Mediator.Send(command);

        return HandleCreatedResult(result);
    }
}