using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Username { get; }
    UserRole? Role { get; }
    bool CanAccessAllRegions { get; }
    bool IsAuthenticated { get; }
    Task<bool> IsActiveAsync(CancellationToken cancellationToken = default);
    
    List<int> GetRegionIds();
    bool HasAccessToRegion(int regionId);
}