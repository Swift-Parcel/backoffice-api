using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Integration.Interfaces;
using SwiftParcel.Application.Integration.Models;
using SwiftParcel.Infrastructure.Authentication;

namespace SwiftParcel.Api.Controllers.Integration;

[ApiController]
[Route("api/integration/parcels")]
[ApiKeyAuth]
public class ParcelsController : ControllerBase
{
    private readonly IParcelService _parcelService;

    public ParcelsController(IParcelService parcelService)
    {
        _parcelService = parcelService;
    }

    private static ErrorResponseDto CreateTrackingNumberNotFoundError(string trackingNumber)
    {
        return new ErrorResponseDto($"Parcel with tracking number {trackingNumber} was not found.");
    }
    
    /// <summary>
    /// Returns current parcel status, location, and tracking history for a given tracking number.
    /// </summary>
    [HttpGet("{trackingNumber}")]
    [ProducesResponseType(typeof(ParcelTrackingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTracking(string trackingNumber, CancellationToken cancellationToken)
    {
        var result = await _parcelService.GetParcelTrackingAsync(trackingNumber, cancellationToken);

        if (result is null)
        {
            return NotFound(CreateTrackingNumberNotFoundError(trackingNumber));
        }

        return Ok(result);
    }
    
    [HttpGet("{trackingNumber}/status")]
    [ProducesResponseType(typeof(ParcelStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatus(string trackingNumber, CancellationToken cancellationToken)
    {
        var result = await _parcelService.GetParcelStatusAsync(trackingNumber, cancellationToken);

        if (result is null)
        {
            return NotFound(CreateTrackingNumberNotFoundError(trackingNumber));
        }

        return Ok(result);
    }

    [HttpGet("{trackingNumber}/delivery-estimate")]
    [ProducesResponseType(typeof(DeliveryEstimateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeliveryEstimate(string trackingNumber, CancellationToken cancellationToken)
    {
        var result = await _parcelService.GetDeliveryEstimateAsync(trackingNumber, cancellationToken);

        if (result is null)
        {
            return NotFound(CreateTrackingNumberNotFoundError(trackingNumber));
        }

        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerParcelDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomerParcels([FromQuery] string customerEmail, CancellationToken cancellationToken)
    {
        var result = await _parcelService.GetCustomerParcelsAsync(customerEmail, cancellationToken);
        
        if (result is null)
        {
            return NotFound(CreateTrackingNumberNotFoundError(customerEmail));
        }
        
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateParcelResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateParcel([FromBody] CreateParcelRequest request, CancellationToken cancellationToken)
    {
        var result = await _parcelService.CreateParcelAsync(request, cancellationToken);
        
        if (result is null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(GetTracking), new { trackingNumber = result.TrackingNumber }, result);
    }

    [HttpPost("{trackingNumber}/delivery-change")]
    [ProducesResponseType(typeof(DeliveryChangeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeDelivery(string trackingNumber, [FromBody] DeliveryChangeRequest request, CancellationToken cancellationToken)
    {
        var result = await _parcelService.ChangeDeliveryAsync(trackingNumber, request, cancellationToken);

        if (result is null)
        {
            return NotFound(CreateTrackingNumberNotFoundError(trackingNumber));
        }

        return Ok(result);
    }

    [HttpPatch("{trackingNumber}/confirm-delivery")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmDelivery(string trackingNumber, CancellationToken cancellationToken)
    {
        var success = await _parcelService.ConfirmDeliveryAsync(trackingNumber, cancellationToken);

        if (!success)
        {
            return NotFound(CreateTrackingNumberNotFoundError(trackingNumber));
        }

        return NoContent();
    }
}