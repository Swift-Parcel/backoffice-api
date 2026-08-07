namespace SwiftParcel.Application.DTO.Parcels;

public record CreateParcelSenderDto(
     string Email,
     AddressDto SenderAddress
);