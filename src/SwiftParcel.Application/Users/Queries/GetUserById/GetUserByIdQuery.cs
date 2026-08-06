using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;

namespace SwiftParcel.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<Result<UserDetailsDto>>;