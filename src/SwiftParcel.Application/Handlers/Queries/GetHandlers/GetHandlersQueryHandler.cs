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
    : IRequestHandler<GetHandlersQuery, Result<PagedList<HandlerDto>>>
{
    public async Task<Result<PagedList<HandlerDto>>> Handle(GetHandlersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<int>? allowedRegionIds = currentUserService.CanAccessAllRegions 
            ? null 
            : currentUserService.GetRegionIds();

        var pagedEntities = await handlerRepository.GetFilteredPagedWithDetailsAsync(
            allowedRegionIds,
            request.IsActive,
            request.Department,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = pagedEntities.Items.Select(h => new HandlerDto(
            h.Id,
            h.UserId,
            h.User?.FullName,
            h.User?.Email,
            h.Department,
            h.MaxCases,
            h.Cases?.Count ?? 0,
            h.HireDate,
            h.IsActive,
            h.User?.Regions?.Select(r => r.Id).ToList() ?? new List<int>()
        )).ToList();

        var pagedDtos = new PagedList<HandlerDto>(
            dtos,
            pagedEntities.TotalCount,
            pagedEntities.PageNumber,
            pagedEntities.PageSize);

        return Result<PagedList<HandlerDto>>.Success(pagedDtos);
    }
}