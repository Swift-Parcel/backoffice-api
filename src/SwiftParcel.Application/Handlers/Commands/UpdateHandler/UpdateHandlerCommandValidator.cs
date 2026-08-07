using FluentValidation;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandler;

public class UpdateHandlerCommandValidator : AbstractValidator<UpdateHandlerCommand>
{
    private readonly IHandlerRepository _handlerRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateHandlerCommandValidator(IHandlerRepository handlerRepository, ICurrentUserService currentUserService)
    {
        _handlerRepository = handlerRepository;
        _currentUserService = currentUserService;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .Must(x => x.Department != null || x.MaxCases > 0) 
            .WithMessage("At least one field (Department or MaxCases) must be provided to update.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department cannot be empty if provided.")
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters.")
            .When(x => x.Department != null);

        RuleFor(x => x.Id)
            .CustomAsync(ValidateHandlerAndRegionsAsync);

        RuleFor(x => x.MaxCases)
            .GreaterThan(0).WithMessage("MaxCases must be greater than zero.")
            .MustAsync(async (command, maxCases, ct) => await NotBeLowerThanCurrentActiveCases(command.Id, maxCases, ct)) 
            .WithMessage("MaxCases cannot be set lower than the handler's currently active cases.")
            .When(x => x.MaxCases > 0);
    }

    private async Task ValidateHandlerAndRegionsAsync(int handlerId, ValidationContext<UpdateHandlerCommand> context, CancellationToken cancellationToken)
    {
        var handler = await _handlerRepository.GetByIdWithUserRegionsAsync(handlerId, cancellationToken);

        if (handler == null)
        {
            context.AddFailure("The specified handler does not exist.");
            return;
        }

        if (!_currentUserService.CanAccessAllRegions)
        {
            bool hasAccess = handler.User.Regions.Any(r => _currentUserService.HasAccessToRegion(r.Id));
            if (!hasAccess)
            {
                context.AddFailure("You do not have permission to modify a handler in this region.");
            }
        }
    }

    private async Task<bool> NotBeLowerThanCurrentActiveCases(int handlerId, int newMaxCases, CancellationToken cancellationToken)
    {
        var activeCasesCount = await _handlerRepository.GetActiveCasesCountAsync(handlerId, cancellationToken);
        return newMaxCases >= activeCasesCount;
    }
}