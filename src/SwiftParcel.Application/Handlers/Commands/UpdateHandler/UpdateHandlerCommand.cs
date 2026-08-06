using MediatR;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandler;

public record UpdateHandlerCommand(
    int Id,
    string? Department,
    int? MaxCases
) : IRequest<Result<Unit>>;