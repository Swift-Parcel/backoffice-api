using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Application.Cases.Queries.GetCaseStatus;
using SwiftParcel.Application.Cases.Queries.GetCustomerCases;
using SwiftParcel.Application.Cases.Commands.AddCaseFeedback;
using SwiftParcel.Application.Cases.Queries.GetCustomerCaseNotes;
using SwiftParcel.Infrastructure.Authentication;

namespace SwiftParcel.Api.Controllers.Integration;

[ApiController]
[Route("api/integration/cases")]
[Produces(MediaTypeNames.Application.Json)]
[ApiKeyAuth]
public class IntegrationCasesController : ApiController
{
    /// <summary>
    /// Return the current status of a case and its resolution.
    /// </summary>
    [HttpGet("{caseNumber}/status")]
    [ProducesResponseType(typeof(CaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCaseStatus(string caseNumber)
    {
        var query = new GetCaseStatusQuery(caseNumber);
        return HandleResult(await Mediator.Send(query));
    }
    
    /// <summary>
    /// Return every case for a customer specified.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CustomerCasesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomerCases([FromQuery] string customerEmail)
    {
        var query = new GetCustomerCasesQuery(customerEmail);
        return HandleResult(await Mediator.Send(query));
    }
    
    /// <summary>
    /// Accept a satisfaction score (1–5) and optional comment after case resolution.
    /// </summary>
    [HttpPost("{caseNumber}/feedback")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCaseFeedback(
        [FromRoute] string caseNumber, 
        [FromBody] AddCaseFeedbackRequest request)
    {
        var command = new AddCaseFeedbackCommand(caseNumber, request.Score);
        return HandleResult(await Mediator.Send(command));
    }
}