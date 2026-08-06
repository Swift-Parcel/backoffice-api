using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Handlers;

namespace SwiftParcel.Application.Handlers.Queries.GetHandlerById;

public record GetHandlerByIdQuery(int Id) : IRequest<Result<HandlerDto>>;