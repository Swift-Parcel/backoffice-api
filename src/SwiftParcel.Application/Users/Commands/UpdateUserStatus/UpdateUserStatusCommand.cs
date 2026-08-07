using MediatR;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Users.Commands.UpdateUserStatus;

public record UpdateUserStatusCommand(int Id, bool IsActive) : IRequest<Result<Unit>>;