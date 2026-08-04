using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.DTO;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Reports.Queries;

public record GetOpenCasesByTypeQuery : IRequest<List<CasesByTypeReportDto>>;

public class GetOpenCasesByTypeQueryHandler : IRequestHandler<GetOpenCasesByTypeQuery, List<CasesByTypeReportDto>>
{
    private readonly IAppDbContext _context;

    public GetOpenCasesByTypeQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CasesByTypeReportDto>> Handle(GetOpenCasesByTypeQuery request, CancellationToken cancellationToken)
    {
        return await _context.Cases
            .Where(c => c.Status != CaseStatus.Closed && c.Status != CaseStatus.Resolved)
            .GroupBy(c => c.CaseType)
            .Select(g => new CasesByTypeReportDto(
                g.Key,
                g.Count()
            ))
            .ToListAsync(cancellationToken);
    }
}