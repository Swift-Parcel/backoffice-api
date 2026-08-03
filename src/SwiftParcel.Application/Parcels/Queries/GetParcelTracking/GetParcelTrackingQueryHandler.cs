using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Application.Helpers;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Infrastructure.Integration.Models;

namespace SwiftParcel.Application.Parcels.Queries.GetParcelTracking;

public class GetParcelTrackingQueryHandler : IRequestHandler<GetParcelTrackingQuery, Result<ParcelTrackingResponse>>
{
    private readonly IAppDbContext _context;

    public GetParcelTrackingQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ParcelTrackingResponse>> Handle(GetParcelTrackingQuery request, CancellationToken cancellationToken)
    {
        var formattedTrackingNumber = FormatHelper.FormatTrackingNumber(request.TrackingNumber);

        var parcel = await _context.Parcels
            .Select(p => new { p.TrackingNumber, p.Status })
            .FirstOrDefaultAsync(p => p.TrackingNumber == formattedTrackingNumber, cancellationToken);

        if (parcel == null)
        {
            return Result<ParcelTrackingResponse>.Failure(
                Error.NotFound("parcel_not_found", $"Parcel with tracking number '{request.TrackingNumber}' was not found."));
        }

        var filePath = Path.Combine(AppContext.BaseDirectory, "Integration", "Mocks", "eurotrack_mock.json");
        
        if (!File.Exists(filePath))
        {
            return Result<ParcelTrackingResponse>.Failure(
                Error.Failure("mock_file_missing", "Eurotrack mock file not found."));
        }

        var jsonString = await File.ReadAllTextAsync(filePath, cancellationToken);
        var mockApiData = JsonSerializer.Deserialize<EuroTrackResponseDto>(jsonString);

        var shipmentData = mockApiData?.Shipments.FirstOrDefault(s =>
            FormatHelper.FormatTrackingNumber(s.TrackingNumber) == formattedTrackingNumber);

        var trackingHistory = new List<TrackingHistoryDto>();
        LocationDto? currentLocation = null;

        var currentParcelStatus = shipmentData != null
            ? MapEuroTrackStatus(shipmentData.CurrentStatus, parcel.Status)
            : parcel.Status;

        if (shipmentData != null && shipmentData.Events.Any())
        {
            var sortedEvents = shipmentData.Events.OrderBy(e => e.Timestamp).ToList();

            foreach (var e in sortedEvents)
            {
                var eventLocation = new LocationDto(
                    Facility: e.Location.Facility,
                    City: e.Location.City,
                    CountryCode: e.Location.CountryCode,
                    PostalCode: e.Location.PostalCode,
                    Lat: e.Location.Lat ?? 0.0,
                    Lon: e.Location.Lon ?? 0.0
                );

                trackingHistory.Add(new TrackingHistoryDto(
                    Timestamp: e.Timestamp,
                    ParcelStatus: MapEuroTrackStatus(e.StatusCode, parcel.Status),
                    Description: e.Description,
                    Location: eventLocation
                ));
            }

            currentLocation = trackingHistory.Last().Location;
        }

        currentLocation ??= new LocationDto(null, "Unknown", "Unknown", "Unknown", 0.0, 0.0);

        return Result<ParcelTrackingResponse>.Success(new ParcelTrackingResponse(
            ParcelStatus: currentParcelStatus,
            Location: currentLocation,
            TrackingHistory: trackingHistory
        ));
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