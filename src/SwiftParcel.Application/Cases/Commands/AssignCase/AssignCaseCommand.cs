using MediatR;
using SwiftParcel.Application.Cases.Dtos;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Commands.AssignCase;

public record AssignCaseCommand(string CaseNumber, int HandlerId) : IRequest<Result<CaseSummaryDto>>, IAuthorizableRequest
{
    public bool RequireAuthentication => true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Operator, UserRole.Supervisor, UserRole.Admin];
};