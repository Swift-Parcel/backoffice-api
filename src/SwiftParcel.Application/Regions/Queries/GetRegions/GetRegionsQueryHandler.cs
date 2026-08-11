using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Regions;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Regions.Queries.GetRegions;

public class GetRegionsQueryHandler(IRegionRepository regionRepository) 
    : IRequestHandler<GetRegionsQuery, Result<PagedList<RegionDto>>>
{
    public async Task<Result<PagedList<RegionDto>>> Handle(GetRegionsQuery request, CancellationToken cancellationToken)
    {
        var (regions, totalCount) = await regionRepository.GetPagedAsync(
            request.NameFilter, 
            request.PageNumber, 
            request.PageSize, 
            cancellationToken);

        var dtos = regions.Select(r => new RegionDto(
            r.Id, 
            r.Name, 
            r.CountryCode, 
            r.BusinessHoursStart, 
            r.BusinessHoursEnd, 
            r.ManagerEmail, 
            r.IsActive)).ToList();

        var pagedResult = new PagedList<RegionDto>
        (
            items : dtos,
            count : totalCount,
            pageNumber : request.PageNumber,
            pageSize : request.PageSize
        );

        return Result<PagedList<RegionDto>>.Success(pagedResult);
    }
}