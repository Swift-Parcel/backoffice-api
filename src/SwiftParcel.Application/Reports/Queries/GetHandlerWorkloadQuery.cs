using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.DTO;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Reports.Queries;

public record GetHandlerWorkloadQuery : IRequest<List<HandlerWorkloadReportDto>>;

public class GetHandlerWorkloadQueryHandler : IRequestHandler<GetHandlerWorkloadQuery, List<HandlerWorkloadReportDto>>
{
    private readonly IAppDbContext _context;

    public GetHandlerWorkloadQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<HandlerWorkloadReportDto>> Handle(GetHandlerWorkloadQuery request, CancellationToken cancellationToken)
    {
        return await _context.Handlers
            .Include(h => h.User)
            .Select(h => new HandlerWorkloadReportDto(
                h.Id,
                h.User.FullName,
                h.Cases.Count(c => c.Status != CaseStatus.Closed && c.Status != CaseStatus.Resolved),
                h.MaxCases
            ))
            .ToListAsync(cancellationToken);
    }
}