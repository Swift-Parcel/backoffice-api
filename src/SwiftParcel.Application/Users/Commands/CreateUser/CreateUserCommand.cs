using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;

namespace SwiftParcel.Application.Users.Commands.CreateUser;

public record CreateUserCommand (
    string Username,
    string Password,
    string FullName,
    int RoleId,
    string Email,
    List<int> RegionIds
) : IRequest<Result<CreateUserResponse>>;