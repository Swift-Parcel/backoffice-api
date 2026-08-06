using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;

namespace SwiftParcel.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IAppDbContext _context;

    public UpdateUserCommandValidator(IAppDbContext context)
    {
        _context = context;

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.FullName) || x.RoleId.HasValue || x.RegionIds != null)
            .WithMessage("At least one field (FullName, RoleId, or RegionIds) must be provided to update.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name cannot be empty if provided.")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.")
            .When(x => x.FullName != null);

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("Role ID must be valid.")
            .MustAsync(RoleExists).WithMessage("The specified Role ID does not exist.")
            .When(x => x.RoleId.HasValue);

        RuleFor(x => x.RegionIds)
            .MustAsync(AllRegionsExist).WithMessage("One or more specified Region IDs do not exist.")
            .When(x => x.RegionIds != null);
    }

    private async Task<bool> RoleExists(int? roleId, CancellationToken cancellationToken)
    {
        if (!roleId.HasValue) return true;
        return await _context.Roles.AnyAsync(r => r.Id == roleId.Value, cancellationToken);
    }

    private async Task<bool> AllRegionsExist(List<int>? regionIds, CancellationToken cancellationToken)
    {
        if (regionIds == null || !regionIds.Any()) return true;
        var count = await _context.Regions.CountAsync(r => regionIds.Contains(r.Id), cancellationToken);
        return count == regionIds.Distinct().Count();
    }
}