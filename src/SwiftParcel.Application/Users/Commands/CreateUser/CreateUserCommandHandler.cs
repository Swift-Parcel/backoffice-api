using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Authentication;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IRegionRepository regionRepository,
    IPasswordHasher passwordHasher,
    ICurrentUserService currentUserService) 
    : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
{
    public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (!await roleRepository.ExistsAsync(request.RoleId, cancellationToken))
            return Result<CreateUserResponse>.Failure(Error.Validation("Role.Invalid", "The specified Role ID does not exist."));

        if (!await userRepository.IsUsernameUniqueAsync(request.Username, cancellationToken))
            return Result<CreateUserResponse>.Failure(Error.Conflict("User.UsernameTaken", "The specified username is already taken."));

        if (!await userRepository.IsEmailUniqueAsync(request.Email, cancellationToken))
            return Result<CreateUserResponse>.Failure(Error.Conflict("User.EmailTaken", "A user with this email already exists."));

        var regions = new List<Region>();
        if (request.RegionIds != null && request.RegionIds.Any())
        {
            var distinctRegionIds = request.RegionIds.Distinct().ToList();
            regions = await regionRepository.GetByIdsAsync(distinctRegionIds, cancellationToken);
            
            if (regions.Count != distinctRegionIds.Count)
            {
                return Result<CreateUserResponse>.Failure(Error.Validation("Region.Invalid", "One or more specified Region IDs do not exist."));
            }
        }

        var newUser = new User
        {
            Username = request.Username,
            PasswordHash = passwordHasher.HashPassword(request.Password), 
            FullName = request.FullName,
            RoleId = request.RoleId,
            Email = request.Email,
            CreatedDate = DateTime.UtcNow,
            CreatedById = (int)currentUserService.UserId,
            Regions = regions
        };
        
        await userRepository.AddAsync(newUser, cancellationToken);
        
        return Result<CreateUserResponse>.Success(new CreateUserResponse(newUser.Id));
    }
}