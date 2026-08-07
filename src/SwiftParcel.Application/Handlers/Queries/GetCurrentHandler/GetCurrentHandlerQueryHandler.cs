using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Handlers;

namespace SwiftParcel.Application.Handlers.Queries.GetCurrentHandler;

public class GetCurrentHandlerQueryHandler(
    IHandlerRepository handlerRepository, 
    ICurrentUserService currentUserService)
    : IRequestHandler<GetCurrentHandlerQuery, Result<HandlerDto>>
{
    public async Task<Result<HandlerDto>> Handle(GetCurrentHandlerQuery request, CancellationToken cancellationToken)
    {
        var handler = await handlerRepository.GetByUserIdWithDetailsAsync(
            (int) currentUserService.UserId, cancellationToken);

        if (handler == null)
        {
            return Result<HandlerDto>.Failure(Error.NotFound("Handler.NotFound", "You do not have a handler profile."));
        }

        var dto = new HandlerDto(
            handler.Id,
            handler.UserId,
            handler.User.FullName,
            handler.User.Email,
            handler.Department,
            handler.MaxCases,
            handler.Cases.Count,
            handler.HireDate,
            handler.IsActive,
            handler.User.Regions.Select(r => r.Id).ToList()
        );

        return Result<HandlerDto>.Success(dto);
    }
}