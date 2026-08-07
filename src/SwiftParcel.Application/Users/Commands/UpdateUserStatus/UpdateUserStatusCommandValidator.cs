using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;

namespace SwiftParcel.Application.Users.Commands.UpdateUserStatus;

public class UpdateUserStatusCommandValidator : AbstractValidator<UpdateUserStatusCommand>
{
    private readonly IAppDbContext _context;

    public UpdateUserStatusCommandValidator(IAppDbContext context)
    {
        _context = context;

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("User ID is required.")
            .MustAsync(UserExists).WithMessage("The specified user does not exist.");
    }

    private async Task<bool> UserExists(int userId, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken);
    }
}