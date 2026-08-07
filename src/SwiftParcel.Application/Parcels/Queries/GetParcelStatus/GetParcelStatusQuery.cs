using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Parcels.Queries.GetParcelStatus;

public record GetParcelStatusQuery(string TrackingNumber) : IRequest<Result<ParcelStatusResponse>>, IAuthorizableRequest
{
    public bool RequireAuthentication => true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.ReadOnly, UserRole.Operator, UserRole.Supervisor, UserRole.Admin];
};