using MediatR;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Users.Commands.ActivateUser;

public record ActivateUserCommand(int Id) : IRequest<Result<Unit>>;