using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO;

public record AddCaseNoteRequest(
    [property: JsonPropertyName("customer_email")] string CustomerEmail,
    [property: JsonPropertyName("message")] string Message
);