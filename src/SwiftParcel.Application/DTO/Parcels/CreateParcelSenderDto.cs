using System.Text.Json.Serialization;
using SwiftParcel.Application.Integration.Models;

namespace SwiftParcel.Application.DTO.Parcels;

public record CreateParcelSenderDto(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("sender_address")] AddressDto SenderAddress
);