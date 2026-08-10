using MediatR;
using SwiftParcel.Application.Cases.Dtos;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.CreateCase;

public record CreateCaseCommand(
    string Title,
    string Description,
    CaseType CaseType,
    CaseStatus CaseStatus,
    string CustomerEmail,
    int? HandlerId,
    int? RegionId,
    Channel Channel,
    IReadOnlyCollection<int> TagIds,
    IReadOnlyCollection<int> ParcelIds,
    Priority Priority = Priority.Low
    ) : IRequest<Result<CreateCaseResponse>>, IAuthorizableRequest
{
    public bool RequireAuthentication = true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Operator, UserRole.Supervisor, UserRole.Admin];
}