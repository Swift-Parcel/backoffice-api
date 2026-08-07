using SwiftParcel.Application.DTO.Parcels;

namespace SwiftParcel.Application.DTO;

public record EuroTrackResponseDto(
    List<EuroTrackShipmentDto> Shipments
);

public record EuroTrackShipmentDto(
    string TrackingNumber,
    string CurrentStatus,
    List<TrackingHistoryEventDto> Events
);

public record EuroTrackEventDto(
    DateTime Timestamp,
    string StatusCode,
    string Description,
    LocationDto Location
);

public record EuroTrackLocation(
    string Facility,
    string City,
    string CountryCode,
    string PostalCode,
    double? Lat,
    double? Lon
);