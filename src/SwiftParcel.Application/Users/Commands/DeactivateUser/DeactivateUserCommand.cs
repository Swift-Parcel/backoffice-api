using MediatR;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Users.Commands.DeactivateUser;

public record DeactivateUserCommand(int Id) : IRequest<Result<Unit>>;