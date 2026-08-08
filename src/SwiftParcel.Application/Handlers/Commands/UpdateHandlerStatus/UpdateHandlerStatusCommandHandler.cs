using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandlerStatus;

public class UpdateHandlerStatusCommandHandler(
    IHandlerRepository handlerRepository,
    ICurrentUserService currentUserService) 
    : IRequestHandler<UpdateHandlerStatusCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(UpdateHandlerStatusCommand request, CancellationToken cancellationToken)
    {
        var handler = await handlerRepository.GetByIdWithUserRegionsAsync(request.Id, cancellationToken);
        if (handler == null)
            return Result<Unit>.Failure(Error.NotFound("The specified handler does not exist."));

        if (!currentUserService.CanAccessAllRegions)
        {
            if (!handler.User.Regions.Any(r => currentUserService.HasAccessToRegion(r.Id)))
                return Result<Unit>.Failure(Error.Forbidden("No permission to change the status of a handler in this region."));
        }

        if (handler.IsActive == request.IsActive)
            return Result<Unit>.Failure(Error.Conflict($"Handler is already {(request.IsActive ? "active" : "deactivated")}."));

        handler.IsActive = request.IsActive;
        await handlerRepository.UpdateAsync(handler, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}