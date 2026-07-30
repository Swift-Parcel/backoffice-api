using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.DTO;

namespace SwiftParcel.Api.Controllers.Integration;


[ApiController]
[Route("api/integration/[controller]")]
public class CasesController : ControllerBase
{
    private readonly ICaseIntegrationService _caseIntegrationService;
    
    public CasesController(ICaseIntegrationService caseIntegrationService)
    {
        _caseIntegrationService = caseIntegrationService;
    }
    
    /// <summary>
    /// Return the current status of a case and its resolution.
    /// </summary>
    [HttpGet("{caseNumber}/status")]
    [ProducesResponseType(typeof(CaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCaseStatus(string caseNumber, CancellationToken cancellationToken)
    {
        var result = await _caseIntegrationService.GetCaseStatusAsync(caseNumber, cancellationToken);

        if (result is null)
        {
            return NotFound(new { message = $"Case with number '{caseNumber}' was not found." });
        }

        return Ok(result);
    }
    
    /// <summary>
    /// Create a new case (complaint) submitted by a customer.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateCaseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCase([FromBody] CreateCaseRequest request, CancellationToken cancellationToken)
    {
        var result = await _caseIntegrationService.CreateCaseAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Accept a customer-visible note/message added to an existing case.
    /// </summary>
    [HttpPost("{caseNumber}/notes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCaseNote(
        [FromRoute] string caseNumber,
        [FromBody] AddCaseNoteRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { message = "Message content cannot be empty." });

        await _caseIntegrationService.AddCaseNoteAsync(caseNumber, request, cancellationToken);
        return Ok();
    }
}