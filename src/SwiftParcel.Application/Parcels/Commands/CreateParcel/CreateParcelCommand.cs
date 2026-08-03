using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;
using SwiftParcel.Application.DTO.Parcels;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Parcels.Commands.CreateParcel;

public record CreateParcelCommand(
    CreateParcelSenderDto Sender,
    CreateParcelRecipientDto Recipient,
    CreateParcelDetailsDto Parcel
) : IRequest<Result<CreateParcelResponse>>;

public record CreateParcelSenderDto(string Email);
public record CreateParcelRecipientDto(string Name, AddressDto RecipientAddress);
public record CreateParcelDetailsDto(
    double Weight,
    double Width,
    double Length,
    double Height,
    ServiceType ServiceType,
    decimal DeclaredValue
);