using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Queries.GetUsers;

public record GetUsersQuery(
    int? RoleId = null,
    bool? IsActive = null,
    string? SearchTerm = null
) : PagedQuery, IRequest<Result<PagedList<UserDetailsDto>>>, IAuthorizableRequest
{
    public bool RequireAuthentication = true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Admin];
}