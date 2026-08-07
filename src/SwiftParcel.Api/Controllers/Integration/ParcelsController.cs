using Microsoft.AspNetCore.Mvc;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Parcels.Commands.ChangeDelivery;
using SwiftParcel.Application.Parcels.Commands.ConfirmDelivery;
using SwiftParcel.Application.Parcels.Queries.GetCustomerParcels;
using SwiftParcel.Application.Parcels.Queries.GetDeliveryEstimate;
using SwiftParcel.Application.Parcels.Queries.GetParcelStatus;
using SwiftParcel.Application.Parcels.Queries.GetParcelTracking;
using SwiftParcel.Infrastructure.Authentication;
using CreateParcelCommand = SwiftParcel.Application.Parcels.Commands.CreateParcel.CreateParcelCommand;

namespace SwiftParcel.Api.Controllers.Integration;

[ApiController]
[Route("api/integration/parcels")]
[ApiKeyAuth]
public class ParcelsController : ApiController
{
    /// <summary>
    /// Returns current parcel status, location, and tracking history for a given tracking number.
    /// </summary>
    [HttpGet("{trackingNumber}")]
    [ProducesResponseType(typeof(ParcelTrackingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTracking(string trackingNumber, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetParcelTrackingQuery(trackingNumber), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{trackingNumber}/status")]
    [ProducesResponseType(typeof(ParcelStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatus(string trackingNumber, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetParcelStatusQuery(trackingNumber), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{trackingNumber}/delivery-estimate")]
    [ProducesResponseType(typeof(DeliveryEstimateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeliveryEstimate(string trackingNumber, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetDeliveryEstimateQuery(trackingNumber), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerParcelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerParcels([FromQuery] string customerEmail, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetCustomerParcelsQuery(customerEmail), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateParcelResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateParcel([FromBody] CreateParcelCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{trackingNumber}/delivery-change")]
    [ProducesResponseType(typeof(DeliveryChangeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeDelivery(string trackingNumber, [FromBody] DeliveryChangeRequest body, CancellationToken cancellationToken)
    {
        var command = new ChangeDeliveryCommand(trackingNumber,body.Date, body.Timeslot);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{trackingNumber}/confirm-delivery")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmDelivery(string trackingNumber, [FromBody] ConfirmDeliveryRequest request, CancellationToken cancellationToken)    {
        var result = await Mediator.Send(new ConfirmDeliveryCommand(trackingNumber, request.CustomerEmail), cancellationToken);
        return HandleResult(result);
    }
}