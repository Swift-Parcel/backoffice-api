using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Reports.Queries.GetOpenCasesByType;

public class GetOpenCasesByTypeQueryHandler : IRequestHandler<GetOpenCasesByTypeQuery, Result<IReadOnlyList<CasesByTypeReportDto>>>
{
    private readonly IAppDbContext _context;

    public GetOpenCasesByTypeQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyList<CasesByTypeReportDto>>> Handle(GetOpenCasesByTypeQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<CasesByTypeReportDto> reports = await _context.Cases
            .Where(c => c.Status != CaseStatus.Closed && c.Status != CaseStatus.Resolved)
            .GroupBy(c => c.CaseType)
            .Select(g => new CasesByTypeReportDto(
                g.Key,
                g.Count()
            ))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<CasesByTypeReportDto>>.Success(reports);
    }
}