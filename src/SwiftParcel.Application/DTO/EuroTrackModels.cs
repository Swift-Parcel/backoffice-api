namespace SwiftParcel.Infrastructure.Integration.Models;

public record EuroTrackResponseDto(
    List<EuroTrackShipmentDto> Shipments
);

public record EuroTrackShipmentDto(
    string TrackingNumber,
    string CurrentStatus,
    List<EuroTrackEventDto> Events
);

public record EuroTrackEventDto(
    DateTime Timestamp,
    string StatusCode,
    string Description,
    EuroTrackLocation Location
);

public record EuroTrackLocation(
    string Facility,
    string City,
    string CountryCode,
    string PostalCode,
    double? Lat,
    double? Lon
);