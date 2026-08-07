using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Commands.CreateUser;

public record CreateUserCommand (
    string Username,
    string Password,
    string FullName,
    int RoleId,
    string Email,
    List<int> RegionIds
) : IRequest<Result<CreateUserResponse>>, IAuthorizableRequest
{
    public bool RequireAuthentication => true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Admin];
};