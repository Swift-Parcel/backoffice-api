using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Cases;

public record CreateCaseRequest(
    [property: JsonPropertyName("customer_email")] string CustomerEmail,
    [property: JsonPropertyName("tracking_numbers")] List<string> TrackingNumbers,
    [property: JsonPropertyName("case_type")] CaseType CaseType,
    [property: JsonPropertyName("case_title")] string CaseTitle,
    [property: JsonPropertyName("region_id")] int RegionId,
    [property: JsonPropertyName("channel")] Channel Channel,
    [property: JsonPropertyName("description")] string Description
);