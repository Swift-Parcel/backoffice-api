using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Parcels;

namespace SwiftParcel.Application.Parcels.Commands.CreateParcel;

public record CreateParcelCommand(
     CreateParcelSenderDto Sender,
     CreateParcelRecipientDto Recipient,
     CreateParcelDetailsDto Parcel
) : IRequest<Result<CreateParcelResponse>>;