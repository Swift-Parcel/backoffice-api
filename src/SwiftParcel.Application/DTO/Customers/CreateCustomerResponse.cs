using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO.Customers;

public record CreateCustomerResponse(
    [property: JsonPropertyName("created_date")] DateTime CreatedDate
);