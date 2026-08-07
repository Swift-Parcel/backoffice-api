using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;

namespace SwiftParcel.Application.Reports.Queries.GetHandlerWorkload;

public class GetHandlerWorkloadQueryHandler : IRequestHandler<GetHandlerWorkloadQuery, Result<IReadOnlyList<HandlerWorkloadReportDto>>>
{
    private readonly IReportRepository _reportRepository;

    public GetHandlerWorkloadQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<Result<IReadOnlyList<HandlerWorkloadReportDto>>> Handle(GetHandlerWorkloadQuery request, CancellationToken cancellationToken)
    {
        var reports = await _reportRepository.GetHandlerWorkloadReportAsync(cancellationToken);

        return Result<IReadOnlyList<HandlerWorkloadReportDto>>.Success(reports);
    }
}