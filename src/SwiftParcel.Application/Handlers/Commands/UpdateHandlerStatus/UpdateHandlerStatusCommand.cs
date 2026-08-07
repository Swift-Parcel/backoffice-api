using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandlerStatus;

public record UpdateHandlerStatusCommand(int Id, bool IsActive) : IRequest<Result<Unit>>;