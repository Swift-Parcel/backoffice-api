using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandler;

public class UpdateHandlerCommandHandler : IRequestHandler<UpdateHandlerCommand, Result>
{
    private readonly IHandlerRepository _handlerRepository;

    public UpdateHandlerCommandHandler(IHandlerRepository handlerRepository)
    {
        _handlerRepository = handlerRepository;
    }

    public async Task<Result> Handle(UpdateHandlerCommand command, CancellationToken cancellationToken)
    {
        var handler = await _handlerRepository.GetByIdAsync(command.Id, cancellationToken);

        if (handler == null)
        {
            // Failsafe in case it was deleted between validation and execution
            return Result.Failure(new Error("Handler.NotFound", "The specified handler no longer exists.", ErrorType.Failure));
        }

        handler.UpdateHandler(
            userId: command.UserId,
            department: command.Department,
            hireDate: command.HireDate,
            maxCases: command.MaxCases,
            isActive: command.IsActive);

        await _handlerRepository.UpdateAsync(handler, cancellationToken);

        return Result.Success();
    }
}