using MediatR;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandlerStatus;

public record UpdateHandlerStatusCommand(int Id, bool IsActive) : IRequest<Result<Unit>>;