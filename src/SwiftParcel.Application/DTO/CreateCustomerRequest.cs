using System.Text.Json.Serialization;

namespace SwiftParcel.Application.DTO;

public record CreateCustomerRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("phone")] string Phone,
    [property: JsonPropertyName("address")] AddressDto Address
);