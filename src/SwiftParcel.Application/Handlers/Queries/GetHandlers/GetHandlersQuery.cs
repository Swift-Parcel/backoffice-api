using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Handlers;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Handlers.Queries.GetHandlers;

public record GetHandlersQuery(
    bool? IsActive = null,
    string? Department = null,
    string? SearchTerm = null) 
    : PagedQuery, IRequest<Result<PagedList<HandlerDto>>>, IAuthorizableRequest
{
    public bool RequireAuthentication = true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Supervisor, UserRole.Admin];
}