using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Regions;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Regions.Queries.GetRegions;

public record GetRegionsQuery(
    string? NameFilter,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedList<RegionDto>>>, IAuthorizableRequest
{
    public bool RequireAuthentication => true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.ReadOnly, UserRole.Operator, UserRole.Supervisor, UserRole.Admin];
}