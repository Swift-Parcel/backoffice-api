using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Cases;

public record CaseStatusResponse(
     CaseStatus CaseStatus,
     List<CaseNoteDto> Notes,
     string? Resolution
);