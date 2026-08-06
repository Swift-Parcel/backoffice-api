namespace SwiftParcel.Application.DTO.Users;

public record UpdateUserRequest(
    string? FullName,
    int? RoleId,
    List<int>? RegionIds
);