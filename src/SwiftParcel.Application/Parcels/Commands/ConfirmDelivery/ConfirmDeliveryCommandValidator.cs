using FluentValidation;

namespace SwiftParcel.Application.Parcels.Commands.ConfirmDelivery;

public class ConfirmDeliveryCommandValidator : AbstractValidator<ConfirmDeliveryCommand>
{
    public ConfirmDeliveryCommandValidator()
    {
        RuleFor(x => x.TrackingNumber)
            .NotEmpty()
            .WithMessage("Tracking number is required.");
    }
}