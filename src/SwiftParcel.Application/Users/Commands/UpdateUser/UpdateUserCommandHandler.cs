using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IRegionRepository regionRepository) 
    : IRequestHandler<UpdateUserCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithRegionsForUpdateAsync(request.Id, cancellationToken);

        if (user == null)
            return Result<Unit>.Failure(Error.NotFound("User.NotFound", $"User with ID {request.Id} not found."));

        if (request.RoleId.HasValue)
        {
            if (!await roleRepository.ExistsAsync(request.RoleId.Value, cancellationToken))
                return Result<Unit>.Failure(Error.Validation("Role.Invalid", "The specified Role ID does not exist."));
        }

        var fetchedRegions = new List<Region>();
        if (request.RegionIds != null && request.RegionIds.Any())
        {
            var distinctRegionIds = request.RegionIds.Distinct().ToList();
            fetchedRegions = await regionRepository.GetByIdsAsync(distinctRegionIds, cancellationToken);
            
            if (fetchedRegions.Count != distinctRegionIds.Count)
            {
                return Result<Unit>.Failure(Error.Validation("Region.Invalid", "One or more specified Region IDs do not exist."));
            }
        }

        List<int>? regionsToApply = request.RegionIds;
        var finalRoleId = request.RoleId ?? user.RoleId;
        var finalRegionCount = regionsToApply != null ? regionsToApply.Count : user.Regions.Count;

        if (request.RoleId.HasValue && (finalRoleId == 1 || finalRoleId == 4) && regionsToApply == null)
        {
            regionsToApply = new List<int>();
            finalRegionCount = 0;
            fetchedRegions = new List<Region>();
        }

        if (finalRoleId == 2 && finalRegionCount != 1) // Operator
        {
            return Result<Unit>.Failure(Error.Validation("User.RegionCount", 
                "Operators must have exactly one region assigned."));
        }
        if (finalRoleId == 3 && finalRegionCount < 1) // Supervisor
        {
            return Result<Unit>.Failure(Error.Validation("User.RegionCount", 
                "Supervisors must have at least one region assigned."));
        }
        
        if (request.FullName != null)
        {
            user.FullName = request.FullName;
        }

        if (request.RoleId.HasValue)
        {
            user.RoleId = request.RoleId.Value;
        }

        if (regionsToApply != null)
        {
            user.Regions = fetchedRegions;
        }

        await userRepository.UpdateAsync(user, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}