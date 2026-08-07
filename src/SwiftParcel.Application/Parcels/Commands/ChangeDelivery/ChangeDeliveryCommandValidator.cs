using FluentValidation;
using SwiftParcel.Application.Common.Validation;

namespace SwiftParcel.Application.Parcels.Commands.ChangeDelivery;

public class ChangeDeliveryCommandValidator : AbstractValidator<ChangeDeliveryCommand>
{
    public ChangeDeliveryCommandValidator()
    {
        RuleFor(x => x.TrackingNumber)
            .NotEmpty().WithMessage("Tracking number is required.")
            .ValidTrackingNumber();

        RuleFor(x => x)
            .Must(x => x.Date.HasValue || x.Timeslot.HasValue)
            .WithMessage("You must provide at least a Date or a Timeslot.");
    }
    
}