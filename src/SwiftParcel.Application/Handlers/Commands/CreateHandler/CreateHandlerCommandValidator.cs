using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;

namespace SwiftParcel.Application.Handlers.Commands.CreateHandler;

public class CreateHandlerCommandValidator : AbstractValidator<CreateHandlerCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateHandlerCommandValidator(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters.");

        RuleFor(x => x.MaxCases)
            .GreaterThan(0).WithMessage("MaxCases must be greater than zero.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID is required.")
            .MustAsync(UserExists).WithMessage("The specified user does not exist.")
            .MustAsync(NotAlreadyBeAHandler).WithMessage("This user is already a handler.")
            .MustAsync(BeInAllowedRegion).WithMessage("You do not have permission to create a handler for this user's region.");
    }

    private async Task<bool> UserExists(int userId, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken);
    }

    private async Task<bool> NotAlreadyBeAHandler(int userId, CancellationToken cancellationToken)
    {
        return !await _context.Handlers.AnyAsync(h => h.UserId == userId, cancellationToken);
    }

    private async Task<bool> BeInAllowedRegion(int userId, CancellationToken cancellationToken)
    {
        if (_currentUserService.CanAccessAllRegions) return true;

        var targetUser = await _context.Users
            .Include(u => u.Regions)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (targetUser == null) return false;

        return targetUser.Regions.Any(r => _currentUserService.HasAccessToRegion(r.Id));
    }
}