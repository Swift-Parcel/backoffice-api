using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Cases;

public record CustomerCaseItemDto(
     string CaseNumber,
     CaseType CaseType,
     CaseStatus CaseStatus,
     DateTime Created,
     DateTime LastUpdate
);