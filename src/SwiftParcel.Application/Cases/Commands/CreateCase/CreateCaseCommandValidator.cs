using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;

namespace SwiftParcel.Application.Cases.Commands.CreateCase;

public class CreateCaseCommandValidator : AbstractValidator<CreateCaseCommand>
{
    public CreateCaseCommandValidator(IAppDbContext context)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(5).WithMessage("Title must be at least 5 characters long.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters long.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.CaseType).IsInEnum().WithMessage("Invalid case type.");
        RuleFor(x => x.CaseStatus).IsInEnum().WithMessage("Invalid case status.");
        RuleFor(x => x.Priority).IsInEnum().WithMessage("Invalid priority.");
        RuleFor(x => x.Channel).IsInEnum().WithMessage("Invalid channel.");

        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Customer email is required.")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(x => x.RegionId)
            .GreaterThan(0).WithMessage("Region ID must be greater than 0.");
        
        RuleFor(x => x.ParcelIds)
            .NotEmpty().WithMessage("At least one parcel is necessary.");
    }
}