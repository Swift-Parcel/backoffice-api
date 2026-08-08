using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Handlers;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Handlers.Queries.GetHandlerById;

public class GetHandlerByIdQueryHandler(
    IHandlerRepository handlerRepository,
    ICurrentUserService currentUserService) 
    : IRequestHandler<GetHandlerByIdQuery, Result<HandlerDto>>
{
    public async Task<Result<HandlerDto>> Handle(GetHandlerByIdQuery request, CancellationToken cancellationToken)
    {
        var handler = await handlerRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (handler == null)
            return Result<HandlerDto>.Failure(Error.NotFound("The specified handler was not found."));

        if (!currentUserService.CanAccessAllRegions)
        {
            var userRegions = currentUserService.GetRegionIds();
            if (!handler.User.Regions.Any(r => userRegions.Contains(r.Id)))
                return Result<HandlerDto>.Failure(Error.Forbidden("You do not have permission to view this handler."));
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