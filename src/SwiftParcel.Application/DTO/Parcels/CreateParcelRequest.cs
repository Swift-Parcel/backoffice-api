using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO.Parcels;

public record CreateParcelRequest(
     CreateParcelSenderDto Sender,
     CreateParcelRecipientDto Recipient,
     CreateParcelDetailsDto Parcel
);