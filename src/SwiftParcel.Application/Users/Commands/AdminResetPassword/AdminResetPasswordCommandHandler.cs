using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Authentication;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Users.Commands.AdminResetPassword;

public class AdminResetPasswordCommandHandler : IRequestHandler<AdminResetPasswordCommand, Result<Unit>>
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public AdminResetPasswordCommandHandler(
        IAppDbContext context,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Unit>> Handle(AdminResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            return Result<Unit>.Failure(Error.NotFound("User.NotFound", $"User with ID {request.UserId} not found."));

        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}