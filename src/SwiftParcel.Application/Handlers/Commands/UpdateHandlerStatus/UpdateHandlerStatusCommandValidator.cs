using FluentValidation;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandlerStatus;

public class UpdateHandlerStatusCommandValidator : AbstractValidator<UpdateHandlerStatusCommand>
{
    public UpdateHandlerStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Handler ID is required.");
    }
}