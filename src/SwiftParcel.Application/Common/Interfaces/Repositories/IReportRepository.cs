namespace SwiftParcel.Application.Common.Interfaces.Repositories;

using DTO;

public interface IReportRepository
{
    Task<IReadOnlyList<AverageResolutionTimeReportDto>> GetAverageResolutionTimeReportAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HandlerWorkloadReportDto>> GetHandlerWorkloadReportAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CasesByTypeReportDto>> GetOpenCasesByTypeReportAsync(CancellationToken cancellationToken = default);
}