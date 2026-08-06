using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;

namespace SwiftParcel.Application.Users.Queries.GetUsers;

public record GetUsersQuery(
    int? RoleId = null,
    bool? IsActive = null,
    string? SearchTerm = null
) : IRequest<Result<List<UserDetailsDto>>>;