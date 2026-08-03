using FluentValidation;

namespace SwiftParcel.Application.Parcels.Commands.ChangeDelivery;

public class ChangeDeliveryCommandValidator : AbstractValidator<ChangeDeliveryCommand>
{
    public ChangeDeliveryCommandValidator()
    {
        RuleFor(x => x.TrackingNumber)
            .NotEmpty().WithMessage("Tracking number is required.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.");

        RuleFor(x => x.Timeslot)
            .NotEmpty().WithMessage("Timeslot is required.");
    }
}