using System.Text.Json.Serialization;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO.Cases;

public record CreateCaseRequest(
     string CustomerEmail,
     List<string> TrackingNumbers,
     int RegionId,
     CaseType CaseType,
     string CaseTitle,
     Channel Channel,
     string Description
);