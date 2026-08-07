using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authentication;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Users.Commands.AdminResetPassword;

public class AdminResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher) 
    : IRequestHandler<AdminResetPasswordCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(AdminResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
            return Result<Unit>.Failure(Error.NotFound("User.NotFound", $"User with ID {request.UserId} not found."));

        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);

        await userRepository.UpdateAsync(user, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}