using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO;

public record CaseNoteDto(
    [property: JsonPropertyName("timestamp")] DateTime Timestamp,
    [property: JsonPropertyName("note")] string Note
);