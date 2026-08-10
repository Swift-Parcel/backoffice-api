using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    int Id,
    string? FullName,
    int? RoleId,
    List<int>? RegionIds
) : IRequest<Result>;