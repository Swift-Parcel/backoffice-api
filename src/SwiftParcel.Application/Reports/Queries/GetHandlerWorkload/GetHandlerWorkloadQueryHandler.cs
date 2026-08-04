using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Reports.Queries.GetHandlerWorkload;

public class GetHandlerWorkloadQueryHandler : IRequestHandler<GetHandlerWorkloadQuery, Result<IReadOnlyList<HandlerWorkloadReportDto>>>
{
    private readonly IAppDbContext _context;

    public GetHandlerWorkloadQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyList<HandlerWorkloadReportDto>>> Handle(GetHandlerWorkloadQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<HandlerWorkloadReportDto> reports = await _context.Handlers
            .Include(h => h.User)
            .Select(h => new HandlerWorkloadReportDto(
                h.Id,
                h.User.FullName,
                h.Cases.Count(c => c.Status != CaseStatus.Closed && c.Status != CaseStatus.Resolved),
                h.MaxCases
            ))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<HandlerWorkloadReportDto>>.Success(reports);
    }
}