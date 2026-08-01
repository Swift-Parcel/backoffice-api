namespace SwiftParcel.Application.DTO.Parcels;

using System.Text.Json.Serialization;

public record ConfirmDeliveryRequest(
     string CustomerEmail
);