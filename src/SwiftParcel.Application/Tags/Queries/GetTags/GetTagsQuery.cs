using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Tags;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Tags.Queries.GetTags;

public record GetTagsQuery(
    string? NameFilter,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedList<TagDto>>>, IAuthorizableRequest
{
    public bool RequireAuthentication => true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.ReadOnly, UserRole.Operator, UserRole.Supervisor, UserRole.Admin];
}