using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Application.Exceptions;
using SwiftParcel.Application.Integration.Models;

namespace SwiftParcel.Api.Controllers.Integration;

[ApiController]
[Route("api/integration/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class CasesController : ControllerBase
{
    private readonly ICaseService _caseService;
    
    public CasesController(ICaseService caseService)
    {
        _caseService = caseService;
    }
    
    private static ErrorResponseDto CreateValidationError(string message)
    {
        return new ErrorResponseDto(message);
    }
    
    /// <summary>
    /// Return the current status of a case and its resolution.
    /// </summary>
    [HttpGet("{caseNumber}/status")]
    [ProducesResponseType(typeof(CaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCaseStatus(string caseNumber, CancellationToken cancellationToken)
    {
        var result = await _caseService.GetCaseStatusAsync(caseNumber, cancellationToken);

        return Ok(result);
    }
    
    /// <summary>
    /// Return every case for a customer specified.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CustomerCasesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerCases([FromQuery] string customerEmail, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();
        
        if (string.IsNullOrWhiteSpace(customerEmail))
        {
            errors.Add(nameof(customerEmail), new[] { "Customer email is required." });
        }
        
        if (errors.Count > 0)
            throw new ValidationException(errors);

        var result = await _caseService.GetCustomerCasesAsync(customerEmail, cancellationToken);
        
        return Ok(result);
    }
    
    /// <summary>
    /// Accept a customer-visible note/message added to an existing case.
    /// </summary>
    [HttpPost("{caseNumber}/notes")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCaseNote(
        [FromRoute] string caseNumber,
        [FromBody] AddCaseNoteRequest request,
        CancellationToken cancellationToken)
    {

        await _caseService.AddCaseNoteAsync(caseNumber, request, cancellationToken);
        return Ok();
    }
    
    /// <summary>
    /// Accept a satisfaction score (1–5) and optional comment after case resolution.
    /// </summary>
    [HttpPost("{caseNumber}/feedback")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCaseFeedback(
        [FromRoute] string caseNumber, 
        [FromBody] AddCaseFeedbackRequest request, 
        CancellationToken cancellationToken)
    {
        if (request.Score < 1 || request.Score > 5)
        {
            return BadRequest(CreateValidationError("Score must be between 1 and 5."));
        }

        await _caseService.AddCaseFeedbackAsync(caseNumber, request, cancellationToken);
        return Ok();
    }
}