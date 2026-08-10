using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Commands.UpdateUserStatus;

public class UpdateUserStatusCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService) 
    : IRequestHandler<UpdateUserStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            return Result.Failure(Error.NotFound("The specified user does not exist."));
        }

        if ((int) currentUserService.UserId == user.Id)
        {
            return Result.Failure(Error.Forbidden("You cannot update the current user."));
        }

        if (user.IsActive == request.IsActive)
        {
            var statusString = request.IsActive ? "active" : "deactivated";
            return Result.Failure(Error.Conflict($"User is already {statusString}."));
        }

        user.IsActive = request.IsActive;
        
        await userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success();
    }
}