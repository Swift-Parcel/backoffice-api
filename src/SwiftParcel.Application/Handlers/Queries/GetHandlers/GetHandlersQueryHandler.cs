using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Handlers;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Handlers.Queries.GetHandlers;

public class GetHandlersQueryHandler : IRequestHandler<GetHandlersQuery, Result<List<HandlerDto>>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetHandlersQueryHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<HandlerDto>>> Handle(GetHandlersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Handlers
            .Include(h => h.User).ThenInclude(u => u.Regions)
            .Include(h => h.Cases)
            .AsNoTracking()
            .AsQueryable();

        if (!_currentUserService.CanAccessAllRegions)
        {
            var userRegions = _currentUserService.GetRegionIds();
            query = query.Where(h => h.User.Regions.Any(r => userRegions.Contains(r.Id)));
        }

        if (request.IsActive.HasValue)
            query = query.Where(h => h.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Department))
            query = query.Where(h => h.Department == request.Department);

        var handlers = await query.ToListAsync(cancellationToken);

        var dtos = handlers.Select(h => new HandlerDto(
            h.Id,
            h.UserId,
            h.User.FullName,
            h.User.Email,
            h.Department,
            h.MaxCases,
            h.Cases.Count(c => Case.ActiveStatuses.Contains(c.Status)),
            h.HireDate,
            h.IsActive,
            h.User.Regions.Select(r => r.Id).ToList()
        )).ToList();

        return Result<List<HandlerDto>>.Success(dtos);
    }
}