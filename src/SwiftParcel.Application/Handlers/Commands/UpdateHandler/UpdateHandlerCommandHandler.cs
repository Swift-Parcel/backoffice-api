using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandler;

public class UpdateHandlerCommandHandler(
    IHandlerRepository handlerRepository,
    ICurrentUserService currentUserService) 
    : IRequestHandler<UpdateHandlerCommand, Result>
{
    public async Task<Result> Handle(UpdateHandlerCommand command, CancellationToken cancellationToken)
    {
        var handler = await handlerRepository.GetByIdWithUserRegionsAsync(command.Id, cancellationToken);
        if (handler == null)
            return Result.Failure(Error.NotFound("Handler.NotFound", "The specified handler does not exist."));

        if (!currentUserService.CanAccessAllRegions)
        {
            if (!handler.User.Regions.Any(r => currentUserService.HasAccessToRegion(r.Id)))
                return Result.Failure(Error.Forbidden("Handler.Forbidden", "No permission to modify a handler in this region."));
        }

        if (command.MaxCases > 0)
        {
            var activeCasesCount = await handlerRepository.GetActiveCasesCountAsync(command.Id, cancellationToken);
            if (command.MaxCases < activeCasesCount)
                return Result.Failure(Error.Validation("Handler.MaxCases", "MaxCases cannot be set lower than the currently active cases."));
        }

        handler.UpdateHandler(
            userId: command.UserId,
            department: command.Department,
            hireDate: command.HireDate,
            maxCases: command.MaxCases,
            isActive: command.IsActive);

        await handlerRepository.UpdateAsync(handler, cancellationToken);

        return Result.Success();
    }
}