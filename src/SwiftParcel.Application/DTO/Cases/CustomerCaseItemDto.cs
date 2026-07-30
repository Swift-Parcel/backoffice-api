using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Cases;

public record CustomerCaseItemDto(
    [property: JsonPropertyName("case_number")] string CaseNumber,
    [property: JsonPropertyName("case_type")] CaseType CaseType,
    [property: JsonPropertyName("case_status")] CaseStatus CaseStatus,
    [property: JsonPropertyName("created")] DateTime Created,
    [property: JsonPropertyName("last_update")] DateTime LastUpdate
);