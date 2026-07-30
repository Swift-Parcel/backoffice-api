using System.Text.Json.Serialization;
using SwiftParcel.Application.Integration.Models;

namespace SwiftParcel.Application.DTO.Parcels;

public record CreateParcelRecipientDto(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("recipient_address")] AddressDto RecipientAddress
);