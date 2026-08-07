using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Handlers;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Handlers.Queries.GetCurrentHandler;

public record GetCurrentHandlerQuery : IRequest<Result<HandlerDto>>;