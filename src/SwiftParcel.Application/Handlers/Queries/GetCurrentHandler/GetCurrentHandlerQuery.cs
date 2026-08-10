using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Handlers;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Handlers.Queries.GetCurrentHandler;

public record GetCurrentHandlerQuery : IRequest<Result<HandlerDto>>, IAuthorizableRequest
{
    public bool RequireAuthentication = true;
    public IReadOnlyList<UserRole> AllowedRoles => [];
}