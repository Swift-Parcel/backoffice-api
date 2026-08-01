using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Cases;

public record CreateCaseRequest(
     string CustomerEmail,
     List<string> TrackingNumbers,
     CaseType CaseType,
     string Description
);