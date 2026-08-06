using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;

namespace SwiftParcel.Application.Users.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserDetailsDto>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserQueryHandler(
        IAppDbContext context, 
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserDetailsDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = (int)_currentUserService.UserId;

        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Regions)
            .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

        if (user == null)
            return Result<UserDetailsDto>.Failure(Error.NotFound("User.NotFound", "Current user not found."));

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