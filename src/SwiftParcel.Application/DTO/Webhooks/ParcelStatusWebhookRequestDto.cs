using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Webhooks;

public record ParcelStatusWebhookRequest(
     string TrackingNumber,
     ParcelStatus ParcelStatus
);