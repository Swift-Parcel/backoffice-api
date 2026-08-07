using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandler;

public record UpdateHandlerCommand(
    int UserId,
    string Department,
    DateTime HireDate,
    int MaxCases,
    bool IsActive,
    int Id = 0) : IRequest<Result>;