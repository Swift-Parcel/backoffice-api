using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO.Cases;

public record CaseNoteDto(
     DateTime Timestamp,
     string Note
);