using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO;

public record CreateCaseRequest(
    [property: JsonPropertyName("customer_email")] string CustomerEmail,
    [property: JsonPropertyName("tracking_numbers")] List<string> TrackingNumbers,
    [property: JsonPropertyName("case_type")] CaseType CaseType,
    [property: JsonPropertyName("description")] string Description
);