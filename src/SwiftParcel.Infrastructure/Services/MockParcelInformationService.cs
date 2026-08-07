using System.Text.Json;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Domain.Shared;
using SwiftParcel.Domain.ValueObjects;

namespace SwiftParcel.Infrastructure.Services;

public class MockParcelInformationService : IParcelInformationService
{
    public async Task<LocationDto?> GetLocationByTrackingNumberAsync(TrackingNumber trackingNumber, CancellationToken cancellationToken = default)
    {
        var shipment = await GetShipmentByTrackingNumberAsync(trackingNumber, cancellationToken);
        var currentLocation = shipment?.Events.OrderByDescending(e => e.Timestamp).FirstOrDefault()?.Location;

        return currentLocation;
    }
    
    public async Task<EuroTrackShipmentDto?> GetShipmentByTrackingNumberAsync(TrackingNumber trackingNumber, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Integration", "Mocks", "eurotrack_mock.json");
        
        if (!File.Exists(filePath))
        {
            throw new InvalidOperationException("Missing mock data about parcels' location.");
        }
        
        var jsonString = await File.ReadAllTextAsync(filePath, cancellationToken);
        var mockApiData = JsonSerializer.Deserialize<EuroTrackResponseDto>(jsonString);

        return mockApiData?.Shipments.FirstOrDefault(s => s.TrackingNumber == trackingNumber.Value);
    }
    
}