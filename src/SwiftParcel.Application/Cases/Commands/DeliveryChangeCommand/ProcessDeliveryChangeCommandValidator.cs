using FluentValidation;

namespace SwiftParcel.Application.Cases.Commands.DeliveryChangeCommand;

public class ProcessDeliveryChangeCommandValidator : AbstractValidator<ProcessDeliveryChangeCommand>
{
    public ProcessDeliveryChangeCommandValidator()
    {
        RuleFor(x => x.CaseNumber)
            .NotEmpty()
            .WithMessage("Case number is required.");

        RuleFor(x => x.Outcome)
            .IsInEnum()
            .WithMessage("Invalid delivery change outcome provided.");
    }
}