using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.Integration.Models;

namespace SwiftParcel.Api.Controllers.Integration;

[ApiController]
[Route("api/integration/[controller]")]
public class ParcelsController : ControllerBase
{
    private readonly IParcelIntegrationService _parcelIntegrationService;

    public ParcelsController(IParcelIntegrationService parcelIntegrationService)
    {
        _parcelIntegrationService = parcelIntegrationService;
    }

    /// <summary>
    /// Returns current parcel status, location, and tracking history for a given tracking number.
    /// </summary>
    [HttpGet("{trackingNumber}")]
    [ProducesResponseType(typeof(ParcelTrackingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTracking(string trackingNumber, CancellationToken cancellationToken)
    {
        var result = await _parcelIntegrationService.GetParcelTrackingAsync(trackingNumber, cancellationToken);

        if (result is null)
        {
            return NotFound(new ErrorResponseDto($"Parcel with tracking number {trackingNumber} was not found."));
        }

        return Ok(result);
    }
    
    [HttpGet("{trackingNumber}/status")]
    [ProducesResponseType(typeof(ParcelTrackingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatus(string trackingNumber, CancellationToken cancellationToken)
    {
        var result = await _parcelIntegrationService.GetParcelStatusAsync(trackingNumber, cancellationToken);

        if (result is null)
        {
            return NotFound(new ErrorResponseDto($"Parcel with tracking number {trackingNumber} was not found."));
        }

        return Ok(result);
    }
}