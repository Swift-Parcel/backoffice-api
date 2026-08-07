using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<Unit>>
{
    private readonly IAppDbContext _context;

    public UpdateUserCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Regions)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null)
            return Result<Unit>.Failure(Error.NotFound("User.NotFound", $"User with ID {request.Id} not found."));

        List<int>? regionsToApply = request.RegionIds;

        var finalRoleId = request.RoleId ?? user.RoleId;
        var finalRegionCount = regionsToApply != null ? regionsToApply.Count : user.Regions.Count;

        if (request.RoleId.HasValue && (finalRoleId == 1 || finalRoleId == 4) && regionsToApply == null)
        {
            regionsToApply = new List<int>();
            finalRegionCount = 0;
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
            user.Regions = regionsToApply.Any()
                ? await _context.Regions.Where(r => regionsToApply.Contains(r.Id)).ToListAsync(cancellationToken)
                : new List<Region>();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}