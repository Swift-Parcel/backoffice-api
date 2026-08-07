using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Parcels.Queries.GetParcelTracking;

public record GetParcelTrackingQuery(string TrackingNumber) : IRequest<Result<ParcelTrackingResponse>>, IAuthorizableRequest
{
public bool RequireAuthentication => true;
public IReadOnlyList<UserRole> AllowedRoles => [UserRole.ReadOnly, UserRole.Operator, UserRole.Supervisor, UserRole.Admin];
}