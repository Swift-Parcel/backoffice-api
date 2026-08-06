using MediatR;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandler;

public record UpdateHandlerCommand(
    int UserId,
    string Department,
    DateTime HireDate,
    int MaxCases,
    bool IsActive,
    int Id = 0) : IRequest<Result>;