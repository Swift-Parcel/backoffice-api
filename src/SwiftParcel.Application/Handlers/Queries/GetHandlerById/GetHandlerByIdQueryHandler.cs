using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Handlers;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Handlers.Queries.GetHandlerById;

public class GetHandlerByIdQueryHandler : IRequestHandler<GetHandlerByIdQuery, Result<HandlerDto>>
{
    private readonly IAppDbContext _context;

    public GetHandlerByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<HandlerDto>> Handle(GetHandlerByIdQuery request, CancellationToken cancellationToken)
    {
        var handler = await _context.Handlers
            .Include(h => h.User)
            .ThenInclude(u => u.Regions)
            .Include(h => h.Cases)
            .AsNoTracking()
            .FirstAsync(h => h.Id == request.Id, cancellationToken);

        var dto = new HandlerDto(
            handler.Id,
            handler.UserId,
            handler.User.FullName,
            handler.User.Email,
            handler.Department,
            handler.MaxCases,
            handler.Cases.Count(c => Case.ActiveStatuses.Contains(c.Status)),
            handler.HireDate,
            handler.IsActive,
            handler.User.Regions.Select(r => r.Id).ToList()
        );

        return Result<HandlerDto>.Success(dto);
    }
}