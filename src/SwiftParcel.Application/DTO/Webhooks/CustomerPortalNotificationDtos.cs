using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO.Webhooks;

public record ParcelStatusNotificationDto(
    [property: JsonPropertyName("tracking_number")] string TrackingNumber, 
    [property: JsonPropertyName("status")] string Status
);

public record CaseStatusNotificationDto(
    [property: JsonPropertyName("case_number")] string CaseNumber, 
    [property: JsonPropertyName("status")] string Status, 
    [property: JsonPropertyName("resolution")] string? Resolution
);

public record DeliveryChangeNotificationDto(
    [property: JsonPropertyName("case_number")] string CaseNumber, 
    [property: JsonPropertyName("delivery_change_request_outcome")] string DeliveryChangeRequestOutcome
);