using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.Common.Models.Authentication;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Authentication.Commands.Login;

public record LoginCommand(
    string Username,
    string Password
    ) : IRequest<Result<AuthenticationResult?>>;