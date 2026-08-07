using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Commands.AdminResetPassword;

public record AdminResetPasswordCommand(
    int UserId,
    string NewPassword
) : IRequest<Result<Unit>>;