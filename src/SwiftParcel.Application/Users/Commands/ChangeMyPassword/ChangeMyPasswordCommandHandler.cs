using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Authentication;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Commands.ChangeMyPassword;

public class ChangeMyPasswordCommandHandler : IRequestHandler<ChangeMyPasswordCommand, Result<Unit>>
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUserService;

    public ChangeMyPasswordCommandHandler(
        IAppDbContext context,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Unit>> Handle(ChangeMyPasswordCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = (int)_currentUserService.UserId;

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

        if (user == null)
            return Result<Unit>.Failure(Error.NotFound("User.NotFound", "User not found."));

        var isOldPasswordValid = _passwordHasher.VerifyPassword(request.OldPassword, user.PasswordHash);
        if (!isOldPasswordValid)
        {
            return Result<Unit>.Failure(Error.Validation("Password.InvalidOldPassword", "The old password provided is incorrect."));
        }

        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}