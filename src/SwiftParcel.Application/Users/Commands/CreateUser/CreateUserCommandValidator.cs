using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;

namespace SwiftParcel.Application.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IAppDbContext _context;

    public CreateUserCommandValidator(IAppDbContext context)
    {
        _context = context;

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
            .MustAsync(BeUniqueUsername).WithMessage("The specified username is already taken.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.")
            .MustAsync(BeUniqueEmail).WithMessage("A user with this email already exists.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("Role ID must be valid.")
            .MustAsync(RoleExists).WithMessage("The specified Role ID does not exist.");
        
        RuleFor(x => x.RegionIds)
            .MustAsync(AllRegionsExist)
            .WithMessage("One or more specified Region IDs do not exist.");
        
        RuleFor(x => x.RegionIds)
            .Must((command, regionIds) => regionIds != null && regionIds.Count == 1)
            .When(x => x.RoleId == 2)
            .WithMessage("Operators can only be assigned to a single region.");

        RuleFor(x => x.RegionIds)
            .NotEmpty()
            .When(x => x.RoleId == 3)
            .WithMessage("Supervisors must be assigned to at least one region.");

        RuleFor(x => x.RegionIds)
            .Empty()
            .When(x => x.RoleId == 1 || x.RoleId == 4)
            .WithMessage("Admins and Read-Only users are globally scoped and should not be assigned specific regions.");
    }
    
    private async Task<bool> AllRegionsExist(List<int> regionIds, CancellationToken cancellationToken)
    {
        if (regionIds == null || !regionIds.Any()) 
            return true;

        var count = await _context.Regions
            .CountAsync(r => regionIds.Contains(r.Id), cancellationToken);

        return count == regionIds.Distinct().Count(); 
    }

    private async Task<bool> BeUniqueUsername(string username, CancellationToken cancellationToken)
    {
        return !await _context.Users.AnyAsync(u => u.Username == username, cancellationToken);
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    private async Task<bool> RoleExists(int roleId, CancellationToken cancellationToken)
    {
        return await _context.Roles.AnyAsync(r => r.Id == roleId, cancellationToken);
    }
}