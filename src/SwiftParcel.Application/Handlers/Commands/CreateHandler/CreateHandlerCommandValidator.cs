using FluentValidation;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Handlers.Commands.CreateHandler;

public class CreateHandlerCommandValidator : AbstractValidator<CreateHandlerCommand>
{
    private readonly IHandlerRepository _handlerRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateHandlerCommandValidator(
        IHandlerRepository handlerRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _handlerRepository = handlerRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters.");

        RuleFor(x => x.MaxCases)
            .GreaterThan(0).WithMessage("MaxCases must be greater than zero.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID is required.")
            .MustAsync(NotAlreadyBeAHandler).WithMessage("This user is already a handler.")
            .CustomAsync(ValidateUserAndRegionsAsync);
    }

    private async Task<bool> NotAlreadyBeAHandler(int userId, CancellationToken cancellationToken)
    {
        return !await _handlerRepository.ExistsByUserIdAsync(userId, cancellationToken);
    }

    private async Task ValidateUserAndRegionsAsync(int userId, ValidationContext<CreateHandlerCommand> context, CancellationToken cancellationToken)
    {
        var targetUser = await _userRepository.GetByIdWithRegionsAsync(userId, cancellationToken);

        if (targetUser == null)
        {
            context.AddFailure("The specified user does not exist.");
            return;
        }

        if (!_currentUserService.CanAccessAllRegions)
        {
            bool hasAccess = targetUser.Regions.Any(r => _currentUserService.HasAccessToRegion(r.Id));
            if (!hasAccess)
            {
                context.AddFailure("You do not have permission to create a handler for this user's region.");
            }
        }
    }
}