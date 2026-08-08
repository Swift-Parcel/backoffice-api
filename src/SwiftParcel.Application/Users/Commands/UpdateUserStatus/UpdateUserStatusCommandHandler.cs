using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Commands.UpdateUserStatus;

public class UpdateUserStatusCommandHandler(IUserRepository userRepository) 
    : IRequestHandler<UpdateUserStatusCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            return Result<Unit>.Failure(Error.NotFound("The specified user does not exist."));
        }

        if (user.IsActive == request.IsActive)
        {
            var statusString = request.IsActive ? "active" : "deactivated";
            return Result<Unit>.Failure(Error.Conflict($"User is already {statusString}."));
        }

        user.IsActive = request.IsActive;
        
        await userRepository.UpdateAsync(user, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}