using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Authentication;

namespace SwiftParcel.Application.Handlers.Queries.GetHandlerById;

public class GetHandlerByIdQueryValidator : AbstractValidator<GetHandlerByIdQuery>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetHandlerByIdQueryValidator(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Handler ID is required.")
            .MustAsync(HandlerExists).WithMessage("The specified handler was not found.")
            .MustAsync(BeInAllowedRegion).WithMessage("You do not have permission to view this handler.");
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

        var userRegions = _currentUserService.GetRegionIds();
        return handler.User.Regions.Any(r => userRegions.Contains(r.Id));
    }
}