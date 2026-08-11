using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Roles;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Roles.Queries.GetRoles;

public class GetRolesQueryHandler(IRoleRepository roleRepository) 
    : IRequestHandler<GetRolesQuery, Result<PagedList<RoleDto>>>
{
    public async Task<Result<PagedList<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var (roles, totalCount) = await roleRepository.GetPagedAsync(
            request.NameFilter, 
            request.PageNumber, 
            request.PageSize, 
            cancellationToken);

        var dtos = roles.Select(r => new RoleDto(
            r.Id, 
            r.Name, 
            r.Description, 
            r.CanAccessAllRegions, 
            r.IsActive)).ToList();

        var pagedResult = new PagedList<RoleDto>
        (
            items : dtos,
            count : totalCount,
            pageNumber : request.PageNumber,
            pageSize : request.PageSize
        );

        return Result<PagedList<RoleDto>>.Success(pagedResult);
    }
}