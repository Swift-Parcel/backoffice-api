using FluentValidation;

namespace SwiftParcel.Application.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("Role ID must be valid.");
        
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
}