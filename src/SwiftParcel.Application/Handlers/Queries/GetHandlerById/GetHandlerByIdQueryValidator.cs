using FluentValidation;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;

namespace SwiftParcel.Application.Handlers.Queries.GetHandlerById;

public class GetHandlerByIdQueryValidator : AbstractValidator<GetHandlerByIdQuery>
{
    private readonly IHandlerRepository _handlerRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetHandlerByIdQueryValidator(IHandlerRepository handlerRepository, ICurrentUserService currentUserService)
    {
        _handlerRepository = handlerRepository;
        _currentUserService = currentUserService;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Handler ID is required.")
            .CustomAsync(ValidateHandlerAndRegionsAsync);
    }

    private async Task ValidateHandlerAndRegionsAsync(int handlerId, ValidationContext<GetHandlerByIdQuery> context, CancellationToken cancellationToken)
    {
        var handler = await _handlerRepository.GetByIdWithUserRegionsAsync(handlerId, cancellationToken);

        if (handler == null)
        {
            context.AddFailure("The specified handler was not found.");
            return;
        }

        if (!_currentUserService.CanAccessAllRegions)
        {
            var userRegions = _currentUserService.GetRegionIds();
            bool hasAccess = handler.User.Regions.Any(r => userRegions.Contains(r.Id));
            
            if (!hasAccess)
            {
                context.AddFailure("You do not have permission to view this handler.");
            }
        }
    }
}