using FluentValidation;

namespace SwiftParcel.Application.Parcels.Commands.CreateParcel;

public class CreateParcelCommandValidator : AbstractValidator<CreateParcelCommand>
{
    public CreateParcelCommandValidator()
    {
        RuleFor(x => x.Sender.Email)
            .NotEmpty().WithMessage("Sender email is required.")
            .EmailAddress().WithMessage("Invalid sender email format.");

        RuleFor(x => x.Recipient.Name)
            .NotEmpty().WithMessage("Recipient name is required.");

        RuleFor(x => x.Parcel.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than 0.");
    }
}