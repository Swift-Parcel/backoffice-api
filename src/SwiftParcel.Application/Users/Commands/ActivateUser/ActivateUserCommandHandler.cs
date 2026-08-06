using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Users.Commands.ActivateUser;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, Result<Unit>>
{
    private readonly IAppDbContext _context;

    public ActivateUserCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null)
            return Result<Unit>.Failure(Error.NotFound("User.NotFound", $"User with ID {request.Id} not found."));

        if (user.IsActive)
            return Result<Unit>.Failure(Error.Conflict("User.AlreadyActivated", "This user is already active."));

        user.IsActive = true;
        
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}