using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;
using SwiftParcel.Domain.ValueObjects;

namespace SwiftParcel.Application.Parcels.Queries.GetParcelTracking;

public class GetParcelTrackingQueryHandler : IRequestHandler<GetParcelTrackingQuery, Result<ParcelTrackingResponse>>
{
    private readonly IParcelRepository _parcelRepository;
    private readonly IParcelInformationService _parcelInformationService;

    public GetParcelTrackingQueryHandler(IParcelRepository parcelRepository, IParcelInformationService parcelInformationService)
    {
        _parcelRepository = parcelRepository;
        _parcelInformationService = parcelInformationService;
    }

    public async Task<Result<ParcelTrackingResponse>> Handle(GetParcelTrackingQuery request, CancellationToken cancellationToken)
    {
        var trackingNumberResult = TrackingNumber.Create(request.TrackingNumber);
        
        if (!trackingNumberResult.IsSuccess)
        {
            return Result<ParcelTrackingResponse>.Failure(trackingNumberResult.Error!);
        }

        var parcelStatus = await _parcelRepository.GetStatusByTrackingNumberAsync(
            trackingNumberResult.Value, 
            cancellationToken);
        
        if (parcelStatus == null)
        {
            return Result<ParcelTrackingResponse>.Failure(
                Error.NotFound("parcel_not_found", $"Parcel with tracking number '{request.TrackingNumber}' was not found."));
        }

        var shipmentData = await _parcelInformationService.GetShipmentByTrackingNumberAsync(trackingNumberResult.Value, cancellationToken);
        
        if (shipmentData == null)
        {
            return Result<ParcelTrackingResponse>.Failure(
                Error.NotFound("shipment_not_found", $"There is no tracking information " +
                                                     $"available for parcel with tracking number {trackingNumberResult.Value}"));
        }
        
        var currentParcelStatus = MapEuroTrackStatus(shipmentData.CurrentStatus, parcelStatus.Value);
        var lastEvent = shipmentData.Events.LastOrDefault();
        if(lastEvent == null)
        {
            return Result<ParcelTrackingResponse>.Failure(
                Error.NotFound("event_not_found", $"No shipment event has been found for parcel with tracking number {trackingNumberResult.Value}"));
        }
        
        var response = new ParcelTrackingResponse(
            ParcelStatus: currentParcelStatus,
            Location: lastEvent.Location,
            TrackingHistory: shipmentData.Events
        );

        return Result<ParcelTrackingResponse>.Success(response);
    }

    private static ParcelStatus MapEuroTrackStatus(string euroTrackStatusCode, ParcelStatus fallbackStatus) =>
        euroTrackStatusCode switch
        {
            "PICKED_UP" => ParcelStatus.PickedUp,
            "ARRIVED_AT_FACILITY" or "DEPARTED_FACILITY" or "IN_TRANSIT" or "ARRIVED_AT_DELIVERY_DEPOT" => ParcelStatus.InTransit,
            "OUT_FOR_DELIVERY" => ParcelStatus.OutForDelivery,
            "DELIVERED" => ParcelStatus.Delivered,
            "DELIVERY_ATTEMPT_FAILED" => ParcelStatus.DeliveryAttemptFailed,
            "EXCEPTION" => ParcelStatus.Damaged,
            "LOST_IN_NETWORK" => ParcelStatus.Lost,
            _ => fallbackStatus
        };
}