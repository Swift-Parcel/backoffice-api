using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO.Cases;

public record CreateCaseResponse(
    [property: JsonPropertyName("case_number")] string CaseNumber
);