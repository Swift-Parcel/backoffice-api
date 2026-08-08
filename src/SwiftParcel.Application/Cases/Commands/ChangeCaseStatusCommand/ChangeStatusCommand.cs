using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.ChangeCaseStatusCommand;

public record ChangeStatusCommand(
    CaseStatus NewStatus,
    string CaseNumber = "") : IRequest<Result<Unit>>, IAuthorizableRequest
{
    public bool RequireAuthentication => true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Operator, UserRole.Supervisor, UserRole.Admin];
};