using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler(IUserRepository userRepository) 
    : IRequestHandler<GetUsersQuery, Result<List<UserDetailsDto>>>
{
    public async Task<Result<List<UserDetailsDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetFilteredWithRegionsAsync(
            request.RoleId, 
            request.IsActive, 
            request.SearchTerm, 
            cancellationToken);

        var dtos = users.Select(user => new UserDetailsDto(
            user.Id,
            user.Username,
            user.Email,
            user.FullName,
            user.RoleId,
            user.IsActive,
            user.Regions.Select(r => r.Id).ToList()
        )).ToList();

        return Result<List<UserDetailsDto>>.Success(dtos);
    }
}