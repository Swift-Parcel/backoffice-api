using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Reports.Queries.GetOpenCasesByType;

public record GetOpenCasesByTypeQuery : IRequest<Result<IReadOnlyList<CasesByTypeReportDto>>>, IAuthorizableRequest
{
    public bool RequireAuthentication => true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Supervisor, UserRole.Admin];
}