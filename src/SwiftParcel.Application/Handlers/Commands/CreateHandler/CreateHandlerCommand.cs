using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Handlers.Commands.CreateHandler;

public record CreateHandlerCommand(
    int UserId,
    string Department,
    int MaxCases,
    DateTime? HireDate
) : IRequest<Result<int>>;