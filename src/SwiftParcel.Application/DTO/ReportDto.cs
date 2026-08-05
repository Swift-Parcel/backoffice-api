using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.DTO;

public record CasesByTypeReportDto(CaseType CaseType, int Count);

public record SlaBreachesReportDto(int CurrentBreaches, int HistoricalBreaches);

public record AverageResolutionTimeReportDto(CaseType CaseType, double AverageHours);

public record HandlerWorkloadReportDto(int HandlerId, string HandlerName, int ActiveCasesCount, int MaxCases);