using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.Users.Commands.ActivateUser;

namespace SwiftParcel.Application.Users.Commands.UpdateUserStatus;

public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand, Result<Unit>>
{
    private readonly IAppDbContext _context;

    public UpdateUserStatusCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstAsync(u => u.Id == request.Id, cancellationToken);

        if (user.IsActive == request.IsActive)
        {
            var statusString = request.IsActive ? "active" : "deactivated";
            return Result<Unit>.Failure(Error.Conflict("User.Status", $"User is already {statusString}."));
        }

        user.IsActive = request.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}