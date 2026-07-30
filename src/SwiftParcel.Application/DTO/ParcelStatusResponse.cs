using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Integration.Models;

public record ParcelStatusResponse(
    [property: JsonPropertyName("parcel_status")] ParcelStatus ParcelStatus
);