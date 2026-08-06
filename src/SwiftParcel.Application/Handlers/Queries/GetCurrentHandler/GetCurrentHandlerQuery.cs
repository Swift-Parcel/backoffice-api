using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Handlers;

namespace SwiftParcel.Application.Handlers.Queries.GetCurrentHandler;

public record GetCurrentHandlerQuery : IRequest<Result<HandlerDto>>;