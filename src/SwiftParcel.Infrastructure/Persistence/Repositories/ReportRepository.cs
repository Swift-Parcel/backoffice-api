namespace SwiftParcel.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using Application.DTO;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;

    public ReportRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AverageResolutionTimeReportDto>> GetAverageResolutionTimeReportAsync(CancellationToken cancellationToken = default)
    {
        var resolvedCases = await _context.Cases
            .Where(c => c.ResolvedDate.HasValue)
            .Select(c => new
            {
                c.CaseType,
                c.CreatedDate,
                ResolvedDate = c.ResolvedDate!.Value
            })
            .ToListAsync(cancellationToken);

        return resolvedCases
            .GroupBy(c => c.CaseType)
            .Select(g => new AverageResolutionTimeReportDto(
                g.Key,
                Math.Round(g.Average(c => (c.ResolvedDate - c.CreatedDate).TotalHours), 2)
            ))
            .ToList();
    }
}