using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO;

public record CaseStatusResponse(
    [property: JsonPropertyName("case_status")] CaseStatus CaseStatus,
    [property: JsonPropertyName("notes")] List<CaseNoteDto> Notes,
    [property: JsonPropertyName("resolution")] string? Resolution
);