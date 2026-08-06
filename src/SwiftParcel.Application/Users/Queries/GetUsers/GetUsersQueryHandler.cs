using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;

namespace SwiftParcel.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<List<UserDetailsDto>>>
{
    private readonly IAppDbContext _context;

    public GetUsersQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<UserDetailsDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users
            .AsNoTracking()
            .Include(u => u.Regions)
            .AsQueryable();

        if (request.RoleId.HasValue)
        {
            query = query.Where(u => u.RoleId == request.RoleId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(u => 
                u.FullName.ToLower().Contains(searchTerm) || 
                u.Email.ToLower().Contains(searchTerm) ||
                u.Username.ToLower().Contains(searchTerm));
        }

        var users = await query.ToListAsync(cancellationToken);

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