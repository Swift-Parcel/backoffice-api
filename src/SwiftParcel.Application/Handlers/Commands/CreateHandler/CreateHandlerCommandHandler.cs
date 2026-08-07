using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Handlers.Commands.CreateHandler;

public class CreateHandlerCommandHandler(
    IHandlerRepository handlerRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUserService) 
    : IRequestHandler<CreateHandlerCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateHandlerCommand request, CancellationToken cancellationToken)
    {
        var targetUser = await userRepository.GetByIdWithRegionsAsync(request.UserId, cancellationToken);
        if (targetUser == null)
            return Result<int>.Failure(Error.NotFound("User.NotFound", "The specified user does not exist."));

        if (!currentUserService.CanAccessAllRegions)
        {
            if (!targetUser.Regions.Any(r => currentUserService.HasAccessToRegion(r.Id)))
                return Result<int>.Failure(Error.Forbidden("User.Forbidden", "No permission to create a handler for this user's region."));
        }

        if (await handlerRepository.ExistsByUserIdAsync(request.UserId, cancellationToken))
            return Result<int>.Failure(Error.Conflict("Handler.Conflict", "This user is already a handler."));

        var newHandler = new Handler(
            userId: request.UserId,
            department: request.Department,
            maxCases: request.MaxCases,
            hireDate: request.HireDate ?? DateTime.UtcNow
        );

        await handlerRepository.AddAsync(newHandler, cancellationToken);

        return Result<int>.Success(newHandler.Id);
    }
}