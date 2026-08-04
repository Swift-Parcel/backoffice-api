namespace SwiftParcel.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Username { get; }
    string? Role { get; }
    bool CanAccessAllRegions { get; }
    bool IsAuthenticated { get; }
    
    List<int> GetRegionIds();
    bool HasAccessToRegion(int regionId);
}