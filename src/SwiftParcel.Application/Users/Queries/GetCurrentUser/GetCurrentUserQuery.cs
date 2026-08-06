using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;

namespace SwiftParcel.Application.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<Result<UserDetailsDto>>;