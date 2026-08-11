using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler(IUserRepository userRepository) 
    : IRequestHandler<GetUsersQuery, Result<PagedList<UserDetailsDto>>>
{
    public async Task<Result<PagedList<UserDetailsDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetPagedFilteredWithRegionsAsync(
            request.RoleId, 
            request.IsActive, 
            request.SearchTerm, 
            request.PageNumber, 
            request.PageSize, 
            cancellationToken);

        var pagedUsers = users.Items.Select(user => new UserDetailsDto(
            user.Id,
            user.Username,
            user.Email,
            user.FullName,
            user.RoleId,
            user.IsActive,
            user.Regions.Select(r => r.Id).ToList()
        )).ToList();

        var pagedDtos = new PagedList<UserDetailsDto>(
            pagedUsers,
            users.TotalCount,
            users.PageNumber,
            users.PageSize);
        
        return Result<PagedList<UserDetailsDto>>.Success(pagedDtos);
    }
}