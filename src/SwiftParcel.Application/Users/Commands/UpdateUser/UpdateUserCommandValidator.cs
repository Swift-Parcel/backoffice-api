using FluentValidation;

namespace SwiftParcel.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.FullName) || x.RoleId.HasValue || x.RegionIds != null)
            .WithMessage("At least one field (FullName, RoleId, or RegionIds) must be provided to update.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name cannot be empty if provided.")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.")
            .When(x => x.FullName != null);

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("Role ID must be valid.")
            .When(x => x.RoleId.HasValue);
    }
}