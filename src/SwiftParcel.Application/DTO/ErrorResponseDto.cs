using System.Text.Json.Serialization;

namespace SwiftParcel.Application.Integration.Models;

public record ErrorResponseDto(
    [property: JsonPropertyName("facility")] string? message
    );