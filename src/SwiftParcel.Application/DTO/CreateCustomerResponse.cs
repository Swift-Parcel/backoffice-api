using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO;

public record CreateCustomerResponse(
    [property: JsonPropertyName("created_date")] DateTime CreatedDate
);