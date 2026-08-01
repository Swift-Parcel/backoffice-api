using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO.Cases;

public record CustomerCasesResponse(
     List<CustomerCaseItemDto> Cases
);