using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Domain.ValueObjects;

namespace SwiftParcel.Application.Common.Interfaces;

public interface IParcelInformationService
{
    Task<LocationDto?> GetLocationByTrackingNumberAsync(TrackingNumber trackingNumber, CancellationToken cancellationToken = default);
    
    Task<EuroTrackShipmentDto?> GetShipmentByTrackingNumberAsync(TrackingNumber trackingNumber, CancellationToken cancellationToken = default);
}