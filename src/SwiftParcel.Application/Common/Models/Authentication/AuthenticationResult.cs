namespace SwiftParcel.Application.Common.Models.Authentication;

public record AuthenticationResult(
    int UserId,
    string Username,
    string Email,
    string Role,
    List<int> RegionIds,
    bool CanAccessAllRegions,
    string Token
);