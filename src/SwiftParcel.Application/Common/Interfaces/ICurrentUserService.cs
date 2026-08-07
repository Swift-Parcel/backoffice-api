using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Username { get; }
    UserRole? Role { get; }
    bool CanAccessAllRegions { get; }
    bool IsAuthenticated { get; }
    
    List<int> GetRegionIds();
    bool HasAccessToRegion(int regionId);
}