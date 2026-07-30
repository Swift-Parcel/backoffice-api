using System.Text.Json.Serialization;
using SwiftParcel.Application.Integration.Models;

namespace SwiftParcel.Application.DTO.Parcels;

public record CustomerParcelSenderDto(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("customer_address")] AddressDto CustomerAddress
);