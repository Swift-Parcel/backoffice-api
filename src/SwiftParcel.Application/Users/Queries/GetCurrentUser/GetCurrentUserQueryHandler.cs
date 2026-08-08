using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler(
    IUserRepository userRepository,
    ICurrentUserService currentUserService) 
    : IRequestHandler<GetCurrentUserQuery, Result<UserDetailsDto>>
{
    public async Task<Result<UserDetailsDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = (int)currentUserService.UserId;

        var user = await userRepository.GetByIdWithRegionsAsync(currentUserId, cancellationToken);

        if (user == null)
            return Result<UserDetailsDto>.Failure(Error.NotFound("Current user not found."));

        var dto = new UserDetailsDto(
            user.Id,
            user.Username,
            user.Email,
            user.FullName,
            user.RoleId,
            user.IsActive,
            user.Regions.Select(r => r.Id).ToList()
        );

        return Result<UserDetailsDto>.Success(dto);
    }
}