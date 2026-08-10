using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Handlers.Commands.CreateHandler;

public record CreateHandlerCommand(
    int UserId,
    string Department,
    int MaxCases,
    DateTime? HireDate
) : IRequest<Result<int>>, IAuthorizableRequest
{
    public bool RequireAuthentication = true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Supervisor, UserRole.Admin];
}