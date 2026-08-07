using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO.Webhooks;

public record ParcelStatusNotificationDto(
    string TrackingNumber, 
    string Status
);

public record CaseStatusNotificationDto(
    string CaseNumber, 
    string Status, 
    string? Resolution
);

public record DeliveryChangeNotificationDto(
    string CaseNumber, 
    string DeliveryChangeRequestOutcome
);