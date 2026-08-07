using FluentValidation;
using SwiftParcel.Application.Common.Validation;

namespace SwiftParcel.Application.Parcels.Queries.GetParcelStatus;

public class GetParcelStatusQueryValidator : AbstractValidator<GetParcelStatusQuery>
{
    public GetParcelStatusQueryValidator()
    {
        RuleFor(x => x.TrackingNumber)
            .NotEmpty().WithMessage("Tracking number is required.")
            .ValidTrackingNumber()
            .WithMessage("Incorrect tracking number format.");
    }
}