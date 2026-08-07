using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;

namespace SwiftParcel.Application.Reports.Queries.GetOpenCasesByType;

public class GetOpenCasesByTypeQueryHandler : IRequestHandler<GetOpenCasesByTypeQuery, Result<IReadOnlyList<CasesByTypeReportDto>>>
{
    private readonly IReportRepository _reportRepository;

    public GetOpenCasesByTypeQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<Result<IReadOnlyList<CasesByTypeReportDto>>> Handle(GetOpenCasesByTypeQuery request, CancellationToken cancellationToken)
    {
        var reports = await _reportRepository.GetOpenCasesByTypeReportAsync(cancellationToken);

        return Result<IReadOnlyList<CasesByTypeReportDto>>.Success(reports);
    }
}