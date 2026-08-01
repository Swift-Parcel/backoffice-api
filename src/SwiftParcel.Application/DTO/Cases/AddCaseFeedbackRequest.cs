using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO.Cases;

public record AddCaseFeedbackRequest(
     string CustomerEmail,
     int Score,
     string? Message
);