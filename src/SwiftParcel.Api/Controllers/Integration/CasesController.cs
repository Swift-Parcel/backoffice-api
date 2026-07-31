using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Application.Integration.Models;

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

    private static ErrorResponseDto CreateCaseNotFoundError(string caseNumber)
    {
        return new ErrorResponseDto($"Case with number '{caseNumber}' was not found.");
    }
    
    private static ErrorResponseDto CreateEmailNotFoundError(string email)
    {
        return new ErrorResponseDto($"Customer with email '{email}' was not found.");
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
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCaseStatus(string caseNumber, CancellationToken cancellationToken)
    {
        var result = await _caseIntegrationService.GetCaseStatusAsync(caseNumber, cancellationToken);

        if (result is null)
        {
            return NotFound(CreateCaseNotFoundError(caseNumber));
        }

        return Ok(result);
    }
    
    /// <summary>
    /// Return every case for a customer specified.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CustomerCasesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerCases([FromQuery] string customerEmail, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(customerEmail))
        {
            return BadRequest(CreateValidationError("Customer email query parameter is required."));
        }

        var result = await _caseIntegrationService.GetCustomerCasesAsync(customerEmail, cancellationToken);
        
        if (result is null)
        {
            return NotFound(CreateEmailNotFoundError(customerEmail));
        }
        
        return Ok(result);
    }
    
    /// <summary>
    /// Create a new case (complaint) submitted by a customer.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateCaseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCase([FromBody] CreateCaseRequest request, CancellationToken cancellationToken)
    {
        var result = await _caseIntegrationService.CreateCaseAsync(request, cancellationToken);
        
        if (result is null)
        {
            return BadRequest(CreateValidationError("Invalid request or customer not found."));
        }
        
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Accept a customer-visible note/message added to an existing case.
    /// </summary>
    [HttpPost("{caseNumber}/notes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCaseNote(
        [FromRoute] string caseNumber,
        [FromBody] AddCaseNoteRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(CreateValidationError("Message content cannot be empty."));
        }

        await _caseIntegrationService.AddCaseNoteAsync(caseNumber, request, cancellationToken);
        return Ok();
    }
    
    /// <summary>
    /// Accept a satisfaction score (1–5) and optional comment after case resolution.
    /// </summary>
    [HttpPost("{caseNumber}/feedback")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCaseFeedback(
        [FromRoute] string caseNumber, 
        [FromBody] AddCaseFeedbackRequest request, 
        CancellationToken cancellationToken)
    {
        if (request.Score < 1 || request.Score > 5)
        {
            return BadRequest(CreateValidationError("Score must be between 1 and 5."));
        }

        await _caseIntegrationService.AddCaseFeedbackAsync(caseNumber, request, cancellationToken);
        return Ok();
    }
}