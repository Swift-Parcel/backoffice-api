using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO.Cases;

public record AddCaseNoteRequest(
     string CustomerEmail,
     string Message
);