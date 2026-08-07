using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Handlers;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Handlers.Queries.GetHandlers;

public class GetHandlersQueryHandler(
    IHandlerRepository handlerRepository, 
    ICurrentUserService currentUserService) 
    : IRequestHandler<GetHandlersQuery, Result<List<HandlerDto>>>
{
    public async Task<Result<List<HandlerDto>>> Handle(GetHandlersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<int>? allowedRegionIds = currentUserService.CanAccessAllRegions 
            ? null 
            : currentUserService.GetRegionIds();

        var handlers = await handlerRepository.GetFilteredWithDetailsAsync(
            allowedRegionIds,
            request.IsActive,
            request.Department,
            cancellationToken);

        var dtos = handlers.Select(h => new HandlerDto(
            h.Id,
            h.UserId,
            h.User.FullName,
            h.User.Email,
            h.Department,
            h.MaxCases,
            h.Cases.Count,
            h.HireDate,
            h.IsActive,
            h.User.Regions.Select(r => r.Id).ToList()
        )).ToList();

        return Result<List<HandlerDto>>.Success(dtos);
    }
}