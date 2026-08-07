using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Common.Interfaces.Authorization;

public interface IAuthorizableRequest
{
    bool RequireAuthentication => true;

    IReadOnlyList<UserRole> AllowedRoles => [];
}