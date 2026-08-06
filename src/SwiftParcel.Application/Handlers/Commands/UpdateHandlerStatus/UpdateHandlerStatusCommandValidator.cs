using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
namespace SwiftParcel.Application.Handlers.Commands.UpdateHandlerStatus;

public class UpdateHandlerStatusCommandValidator : AbstractValidator<UpdateHandlerStatusCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateHandlerStatusCommandValidator(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;

        RuleFor(x => x.Id)
            .MustAsync(HandlerExists).WithMessage("The specified handler does not exist.")
            .MustAsync(BeInAllowedRegion).WithMessage("You do not have permission to change the status of a handler in this region.");
    }

    private async Task<bool> HandlerExists(int handlerId, CancellationToken cancellationToken)
    {
        return await _context.Handlers.AnyAsync(h => h.Id == handlerId, cancellationToken);
    }

    private async Task<bool> BeInAllowedRegion(int handlerId, CancellationToken cancellationToken)
    {
        if (_currentUserService.CanAccessAllRegions) return true;

        var handler = await _context.Handlers
            .Include(h => h.User)
            .ThenInclude(u => u.Regions)
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == handlerId, cancellationToken);

        if (handler == null) return false;

        return handler.User.Regions.Any(r => _currentUserService.HasAccessToRegion(r.Id));
    }
}