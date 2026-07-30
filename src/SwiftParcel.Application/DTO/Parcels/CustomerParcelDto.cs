using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Parcels;

public record CustomerParcelDto(
    [property: JsonPropertyName("tracking_number")] string TrackingNumber,
    [property: JsonPropertyName("parcel_status")] ParcelStatus ParcelStatus,
    [property: JsonPropertyName("sender")] CustomerParcelSenderDto Sender,
    [property: JsonPropertyName("recipient")] CustomerParcelRecipientDto Recipient,
    [property: JsonPropertyName("created_date")] DateTime CreatedDate,
    [property: JsonPropertyName("service_type")] ServiceType ServiceType
);