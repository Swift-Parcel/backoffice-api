using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(IUserRepository userRepository) 
    : IRequestHandler<GetUserByIdQuery, Result<UserDetailsDto>>
{
    public async Task<Result<UserDetailsDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithRegionsAsync(request.Id, cancellationToken);

        if (user == null)
            return Result<UserDetailsDto>.Failure(Error.NotFound($"User with ID {request.Id} not found."));

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