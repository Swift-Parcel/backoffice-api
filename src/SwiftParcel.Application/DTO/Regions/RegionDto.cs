namespace SwiftParcel.Application.DTO.Regions;

public record RegionDto(
    int Id, 
    string Name, 
    string CountryCode, 
    TimeOnly BusinessHoursStart, 
    TimeOnly BusinessHoursEnd, 
    string ManagerEmail, 
    bool IsActive);