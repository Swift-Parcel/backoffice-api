using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Authentication;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUserService;

    public CreateUserCommandHandler(
        IAppDbContext context,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var regions = new List<Region>();
        if (request.RegionIds != null && request.RegionIds.Any())
        {
            regions = await _context.Regions
                .Where(r => request.RegionIds.Contains(r.Id))
                .ToListAsync(cancellationToken);
        }

        var newUser = new User
        {
            Username = request.Username,
            PasswordHash = _passwordHasher.HashPassword(request.Password), 
            FullName = request.FullName,
            RoleId = request.RoleId,
            Email = request.Email,
            CreatedDate = DateTime.UtcNow,
            CreatedById = (int) _currentUserService.UserId,
            Regions = regions
        };
        
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result<CreateUserResponse>.Success(new CreateUserResponse(newUser.Id));
    }
}