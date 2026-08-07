using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Authentication;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Commands.ChangeMyPassword;

public class ChangeMyPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ICurrentUserService currentUserService) 
    : IRequestHandler<ChangeMyPasswordCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ChangeMyPasswordCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = (int)currentUserService.UserId;

        var user = await userRepository.GetByIdAsync(currentUserId, cancellationToken);

        if (user == null)
        {
            return Result<Unit>.Failure(Error.NotFound("User.NotFound", "User not found."));
        }

        var isOldPasswordValid = passwordHasher.VerifyPassword(request.OldPassword, user.PasswordHash);
        if (!isOldPasswordValid)
        {
            return Result<Unit>.Failure(Error.Validation("Password.InvalidOldPassword", "The old password provided is incorrect."));
        }

        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);

        await userRepository.UpdateAsync(user, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}