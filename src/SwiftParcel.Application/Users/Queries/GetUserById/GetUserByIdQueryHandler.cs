using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;

namespace SwiftParcel.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDetailsDto>>
{
    private readonly IAppDbContext _context;

    public GetUserByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UserDetailsDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Regions)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null)
            return Result<UserDetailsDto>.Failure(Error.NotFound("User.NotFound", $"User with ID {request.Id} not found."));

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