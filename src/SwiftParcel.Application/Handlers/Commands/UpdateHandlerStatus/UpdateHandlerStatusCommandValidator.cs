using FluentValidation;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandlerStatus;

public class UpdateHandlerStatusCommandValidator : AbstractValidator<UpdateHandlerStatusCommand>
{
    private readonly IHandlerRepository _handlerRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateHandlerStatusCommandValidator(IHandlerRepository handlerRepository, ICurrentUserService currentUserService)
    {
        _handlerRepository = handlerRepository;
        _currentUserService = currentUserService;

        RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Handler ID is required.")
            .CustomAsync(ValidateHandlerAndRegionsAsync);
    }

    private async Task ValidateHandlerAndRegionsAsync(int handlerId, ValidationContext<UpdateHandlerStatusCommand> context, CancellationToken cancellationToken)
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
                context.AddFailure("You do not have permission to change the status of a handler in this region.");
            }
        }
    }
}