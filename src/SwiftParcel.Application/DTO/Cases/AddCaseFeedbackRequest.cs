using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO.Cases;

public record AddCaseFeedbackRequest(
    [property: JsonPropertyName("customer_email")] string CustomerEmail,
    [property: JsonPropertyName("score")] int Score,
    [property: JsonPropertyName("message")] string? Message
);