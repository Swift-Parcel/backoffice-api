using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Parcels.Commands.CreateParcel;

public record CreateParcelCommand(
     CreateParcelSenderDto Sender,
     CreateParcelRecipientDto Recipient,
     CreateParcelDetailsDto Parcel
) : IRequest<Result<CreateParcelResponse>>, IAuthorizableRequest

{
     public bool RequireAuthentication => true;
     public IReadOnlyList<UserRole> AllowedRoles => [UserRole.Operator, UserRole.Supervisor, UserRole.Admin];
};