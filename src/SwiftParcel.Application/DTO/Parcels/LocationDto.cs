namespace SwiftParcel.Application.DTO.Parcels;

public record LocationDto(
     string? Facility,
     string? City,
     string? CountryCode,
     string? PostalCode,
     double? Lat,
     double? Lon
);