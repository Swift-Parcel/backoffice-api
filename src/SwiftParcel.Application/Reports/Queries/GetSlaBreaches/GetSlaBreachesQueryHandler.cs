using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.DTO;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Reports.Queries;

public class GetSlaBreachesQueryHandler : IRequestHandler<GetSlaBreachesQuery, SlaBreachesReportDto>
{
    private readonly IAppDbContext _context;

    public GetSlaBreachesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<SlaBreachesReportDto> Handle(GetSlaBreachesQuery request, CancellationToken cancellationToken)
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

        return new SlaBreachesReportDto(currentBreaches, historicalBreaches);
    }
}