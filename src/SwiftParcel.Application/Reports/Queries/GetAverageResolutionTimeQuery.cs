using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.DTO;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Reports.Queries;

public record GetAverageResolutionTimeQuery : IRequest<List<AverageResolutionTimeReportDto>>;

public class GetAverageResolutionTimeQueryHandler : IRequestHandler<GetAverageResolutionTimeQuery, List<AverageResolutionTimeReportDto>>
{
    private readonly IAppDbContext _context;

    public GetAverageResolutionTimeQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AverageResolutionTimeReportDto>> Handle(GetAverageResolutionTimeQuery request, CancellationToken cancellationToken)
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

        return resolvedCases
            .GroupBy(c => c.CaseType)
            .Select(g => new AverageResolutionTimeReportDto(
                g.Key,
                Math.Round(g.Average(c => (c.ResolvedDate - c.CreatedDate).TotalHours), 2)
            ))
            .ToList();
    }
}