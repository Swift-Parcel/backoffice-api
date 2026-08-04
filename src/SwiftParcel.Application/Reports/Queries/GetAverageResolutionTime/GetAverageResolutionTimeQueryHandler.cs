using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;

namespace SwiftParcel.Application.Reports.Queries.GetAverageResolutionTime;

public class GetAverageResolutionTimeQueryHandler : IRequestHandler<GetAverageResolutionTimeQuery, Result<IReadOnlyList<AverageResolutionTimeReportDto>>>
{
    private readonly IAppDbContext _context;

    public GetAverageResolutionTimeQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyList<AverageResolutionTimeReportDto>>> Handle(GetAverageResolutionTimeQuery request, CancellationToken cancellationToken)
    {
        var resolvedCases = await _context.Cases
            .Where(c => c.ResolvedDate.HasValue)
            .Select(c => new
            {
                c.CaseType,
                CreatedDate = c.CreatedDate,
                ResolvedDate = c.ResolvedDate!.Value
            })
            .ToListAsync(cancellationToken);

        IReadOnlyList<AverageResolutionTimeReportDto> reports = resolvedCases
            .GroupBy(c => c.CaseType)
            .Select(g => new AverageResolutionTimeReportDto(
                g.Key,
                Math.Round(g.Average(c => (c.ResolvedDate - c.CreatedDate).TotalHours), 2)
            ))
            .ToList();

        return Result<IReadOnlyList<AverageResolutionTimeReportDto>>.Success(reports);
    }
}