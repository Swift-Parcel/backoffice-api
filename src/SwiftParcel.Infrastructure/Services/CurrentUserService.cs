using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var subClaim = user?
                               .FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? user?.FindFirstValue("sub");
            return int.TryParse(subClaim, out var id) ? id : null;
        }
    }

    public string? Username => _httpContextAccessor.HttpContext?
        .User?.FindFirstValue(ClaimTypes.Name);

    public UserRole? Role => _httpContextAccessor.HttpContext?
        .User?.FindFirstValue(ClaimTypes.Role) is string role ? Enum.Parse<UserRole>(role) : null;

    public bool CanAccessAllRegions => bool.TryParse(
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("can_access_all_regions"),
        out var canAccess) && canAccess;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?
        .User?.Identity?.IsAuthenticated ?? false;

    public List<int> GetRegionIds()
    {
        return _httpContextAccessor?.HttpContext?.User?
            .FindAll("region_id")
            .Select(c => int.TryParse(c.Value, out var id) ? id : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id.Value)
            .ToList() ?? new List<int>();
    }
    
    public bool HasAccessToRegion(int regionId)
    {
        if (CanAccessAllRegions)
            return true;

        return GetRegionIds().Contains(regionId);
    }
}