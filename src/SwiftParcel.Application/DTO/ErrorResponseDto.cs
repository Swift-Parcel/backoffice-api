using System.Text.Json.Serialization;

namespace SwiftParcel.Application.Integration.Models;

public record ErrorResponseDto(
     string? message
    );