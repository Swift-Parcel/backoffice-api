using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Parcels;

public record CustomerParcelDto(
     string TrackingNumber,
     ParcelStatus ParcelStatus,
     CustomerParcelSenderDto Sender,
     CustomerParcelRecipientDto Recipient,
     DateTime CreatedDate,
     ServiceType ServiceType
);