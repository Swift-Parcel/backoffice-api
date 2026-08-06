using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
namespace SwiftParcel.Application.Handlers.Commands.UpdateHandlerStatus;

public class UpdateHandlerStatusCommandHandler : IRequestHandler<UpdateHandlerStatusCommand, Result<Unit>>
{
    private readonly IAppDbContext _context;

    public UpdateHandlerStatusCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(UpdateHandlerStatusCommand request, CancellationToken cancellationToken)
    {
        var handler = await _context.Handlers
            .FirstAsync(h => h.Id == request.Id, cancellationToken);

        if (handler.IsActive == request.IsActive)
        {
            return Result<Unit>.Failure(Error.Conflict("Handler.Status", $"Handler is already {(request.IsActive ? "active" : "deactivated")}."));
        }

        handler.IsActive = request.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}