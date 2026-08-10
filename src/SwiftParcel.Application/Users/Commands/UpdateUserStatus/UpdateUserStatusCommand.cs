using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Commands.UpdateUserStatus;

public record UpdateUserStatusCommand(int Id, bool IsActive) : IRequest<Result>;