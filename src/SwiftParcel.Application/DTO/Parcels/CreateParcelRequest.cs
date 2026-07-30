using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO.Parcels;

public record CreateParcelRequest(
    [property: JsonPropertyName("sender")] CreateParcelSenderDto Sender,
    [property: JsonPropertyName("recipient")] CreateParcelRecipientDto Recipient,
    [property: JsonPropertyName("parcel")] CreateParcelDetailsDto Parcel
);