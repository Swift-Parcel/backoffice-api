namespace SwiftParcel.Application.DTO.Handlers;

public record HandlerDto(
    int Id,
    int UserId,
    string FullName,
    string Email,
    string Department,
    int MaxCases,
    int ActiveCasesCount,
    DateTime HireDate,
    bool IsActive,
    List<int> RegionIds
);