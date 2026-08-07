using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandlerStatus;

public class UpdateHandlerStatusCommandHandler : IRequestHandler<UpdateHandlerStatusCommand, Result<Unit>>
{
    private readonly IHandlerRepository _handlerRepository;

    public UpdateHandlerStatusCommandHandler(IHandlerRepository handlerRepository)
    {
        _handlerRepository = handlerRepository;
    }

    public async Task<Result<Unit>> Handle(UpdateHandlerStatusCommand request, CancellationToken cancellationToken)
    {
        var handler = await _handlerRepository.GetByIdAsync(request.Id, cancellationToken);

        if (handler == null)
        {
            return Result<Unit>.Failure(Error.NotFound("Handler.NotFound", "The specified handler does not exist."));
        }

        if (handler.IsActive == request.IsActive)
        {
            return Result<Unit>.Failure(Error.Conflict("Handler.Status", $"Handler is already {(request.IsActive ? "active" : "deactivated")}."));
        }

        handler.IsActive = request.IsActive;
        
        await _handlerRepository.UpdateAsync(handler, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}