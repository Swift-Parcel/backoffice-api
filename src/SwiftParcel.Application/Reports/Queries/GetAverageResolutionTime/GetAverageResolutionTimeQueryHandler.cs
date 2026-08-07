using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;

namespace SwiftParcel.Application.Reports.Queries.GetAverageResolutionTime;

public class GetAverageResolutionTimeQueryHandler : IRequestHandler<GetAverageResolutionTimeQuery, Result<IReadOnlyList<AverageResolutionTimeReportDto>>>
{
    private readonly IReportRepository _reportRepository;

    public GetAverageResolutionTimeQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<Result<IReadOnlyList<AverageResolutionTimeReportDto>>> Handle(GetAverageResolutionTimeQuery request, CancellationToken cancellationToken)
    {
        var reports = await _reportRepository.GetAverageResolutionTimeReportAsync(cancellationToken);

        return Result<IReadOnlyList<AverageResolutionTimeReportDto>>.Success(reports);
    }
}