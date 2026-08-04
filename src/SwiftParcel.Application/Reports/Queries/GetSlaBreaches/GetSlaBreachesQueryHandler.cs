using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Reports.Queries.GetSlaBreaches;

public class GetSlaBreachesQueryHandler : IRequestHandler<GetSlaBreachesQuery, Result<SlaBreachesReportDto>>
{
    private readonly IAppDbContext _context;

    public GetSlaBreachesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SlaBreachesReportDto>> Handle(GetSlaBreachesQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var currentBreaches = await _context.Cases
            .Where(c => c.Status != CaseStatus.Closed && c.Status != CaseStatus.Resolved)
            .Where(c => c.SlaDeadline < now)
            .CountAsync(cancellationToken);

        var historicalBreaches = await _context.Cases
            .Where(c => c.Status == CaseStatus.Closed || c.Status == CaseStatus.Resolved)
            .Where(c => (c.ResolvedDate ?? c.UpdatedDate) > c.SlaDeadline)
            .CountAsync(cancellationToken);

        var dto = new SlaBreachesReportDto(currentBreaches, historicalBreaches);

        return Result<SlaBreachesReportDto>.Success(dto);
    }
}