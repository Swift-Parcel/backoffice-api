using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Users.Commands.DeactivateUser;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result<Unit>>
{
    private readonly IAppDbContext _context;

    public DeactivateUserCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null)
            return Result<Unit>.Failure(Error.NotFound("User.NotFound", $"User with ID {request.Id} not found."));

        if (!user.IsActive)
            return Result<Unit>.Failure(Error.Conflict("User.AlreadyDeactivated", "This user is already deactivated."));

        user.IsActive = false;
        
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}