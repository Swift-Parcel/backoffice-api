using System.Text.Json.Serialization;
using SwiftParcel.Application.Integration.Models;

namespace SwiftParcel.Application.DTO.Parcels;

public record CustomerParcelRecipientDto(
     string Name,
     AddressDto RecipientAddress
);