using FluentValidation;
using SwiftParcel.Application.Common.Validation;

namespace SwiftParcel.Application.Parcels.Commands.ConfirmDelivery;

public class ConfirmDeliveryCommandValidator : AbstractValidator<ConfirmDeliveryCommand>
{
    public ConfirmDeliveryCommandValidator()
    {
        RuleFor(x => x.TrackingNumber)
            .NotEmpty()
            .WithMessage("Tracking number is required.")
            .ValidTrackingNumber()
            .WithMessage("Invalid tracking number format.");
    }
}