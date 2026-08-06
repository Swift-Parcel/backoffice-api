namespace SwiftParcel.Application.DTO.Users;

public record UserDetailsDto(
    int Id,
    string Username,
    string Email,
    string FullName,
    int RoleId,
    bool IsActive,
    List<int> RegionIds
);