using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Commands.UpdateCaseStatusCommand;

public record UpdateCaseStatusCommand(string CaseNumber, CaseStatus NewStatus) : IRequest<Result<Unit>>, IAuthorizableRequest
{
    public bool RequireAuthentication => true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Operator, UserRole.Supervisor, UserRole.Admin];
};