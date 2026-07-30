using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO;

public record CustomerCasesResponse(
    [property: JsonPropertyName("cases")] List<CustomerCaseItemDto> Cases
);