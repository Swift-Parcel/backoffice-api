using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Handlers;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Handlers.Queries.GetCurrentHandler;

public class GetCurrentHandlerQueryHandler(IAppDbContext context, ICurrentUserService currentUserService)
    : IRequestHandler<GetCurrentHandlerQuery, Result<HandlerDto>>
{
    public async Task<Result<HandlerDto>> Handle(GetCurrentHandlerQuery request, CancellationToken cancellationToken)
    {
        var handler = await context.Handlers
            .Include(h => h.User).ThenInclude(u => u.Regions)
            .Include(h => h.Cases)
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.UserId == currentUserService.UserId, cancellationToken);

        if (handler == null)
            return Result<HandlerDto>.Failure(Error.NotFound("Handler.NotFound", "You do not have a handler profile."));

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