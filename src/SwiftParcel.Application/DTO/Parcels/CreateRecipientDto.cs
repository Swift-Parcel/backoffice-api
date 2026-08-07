namespace SwiftParcel.Application.DTO.Parcels;

public record CreateParcelRecipientDto(
     string Name,
     AddressDto RecipientAddress
);