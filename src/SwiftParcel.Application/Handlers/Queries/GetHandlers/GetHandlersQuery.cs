using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Handlers;
namespace SwiftParcel.Application.Handlers.Queries.GetHandlers;

public record GetHandlersQuery(bool? IsActive = null, string? Department = null) : IRequest<Result<List<HandlerDto>>>;