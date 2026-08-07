using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Commands.ChangeMyPassword;

public record ChangeMyPasswordCommand(
    string OldPassword,
    string NewPassword
) : IRequest<Result<Unit>>;