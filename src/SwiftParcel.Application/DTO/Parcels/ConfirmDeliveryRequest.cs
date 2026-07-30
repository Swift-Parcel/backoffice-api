namespace SwiftParcel.Application.DTO.Parcels;

using System.Text.Json.Serialization;

public record ConfirmDeliveryRequest(
    [property: JsonPropertyName("customer_email")] string CustomerEmail
);