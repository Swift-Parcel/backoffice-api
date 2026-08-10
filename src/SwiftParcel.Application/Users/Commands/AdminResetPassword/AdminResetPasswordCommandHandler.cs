using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authentication;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Commands.AdminResetPassword;

public class AdminResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher) 
    : IRequestHandler<AdminResetPasswordCommand, Result>
{
    public async Task<Result> Handle(AdminResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
            return Result.Failure(Error.NotFound($"User with ID {request.UserId} not found."));

        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);

        await userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success();
    }
}