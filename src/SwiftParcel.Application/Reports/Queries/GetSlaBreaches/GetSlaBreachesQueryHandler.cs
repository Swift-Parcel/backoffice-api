using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Reports.Queries.GetSlaBreaches;

public class GetSlaBreachesQueryHandler : IRequestHandler<GetSlaBreachesQuery, Result<SlaBreachesReportDto>>
{
    private readonly IReportRepository _reportRepository;

    public GetSlaBreachesQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<Result<SlaBreachesReportDto>> Handle(GetSlaBreachesQuery request, CancellationToken cancellationToken)
    {
        var report = await _reportRepository.GetSlaBreachesReportAsync(cancellationToken);

        return Result<SlaBreachesReportDto>.Success(report);
    }
}