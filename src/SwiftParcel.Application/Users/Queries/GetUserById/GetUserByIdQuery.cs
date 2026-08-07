using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Users;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<Result<UserDetailsDto>>;