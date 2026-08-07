using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Parcels.Commands.ConfirmDelivery;

public record ConfirmDeliveryCommand(string TrackingNumber, string CustomerEmail)
    : IRequest<Result<bool>>, IAuthorizableRequest
{
    public bool RequireAuthentication => true;
    public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Operator, UserRole.Supervisor, UserRole.Admin,];
};