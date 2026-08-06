using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandler;

public class UpdateHandlerCommandValidator : AbstractValidator<UpdateHandlerCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateHandlerCommandValidator(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;

        RuleFor(x => x)
            .Must(x => x.Department != null || x.MaxCases.HasValue)
            .WithMessage("At least one field (Department or MaxCases) must be provided to update.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department cannot be empty if provided.")
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters.")
            .When(x => x.Department != null);

        RuleFor(x => x.Id)
            .MustAsync(HandlerExists).WithMessage("The specified handler does not exist.")
            .MustAsync(BeInAllowedRegion).WithMessage("You do not have permission to modify a handler in this region.");

        RuleFor(x => x.MaxCases)
            .GreaterThan(0).WithMessage("MaxCases must be greater than zero.")
            .MustAsync(async (command, maxCases, ct) => await NotBeLowerThanCurrentActiveCases(command.Id, maxCases!.Value, ct))
            .WithMessage("MaxCases cannot be set lower than the handler's currently active cases.")
            .When(x => x.MaxCases.HasValue);
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

    private async Task<bool> NotBeLowerThanCurrentActiveCases(int handlerId, int newMaxCases, CancellationToken cancellationToken)
    {
        var activeCasesCount = await _context.Cases
            .CountAsync(c => c.HandlerId == handlerId && 
                             Case.ActiveStatuses.Contains(c.Status),
                cancellationToken);

        return newMaxCases >= activeCasesCount;
    }
}