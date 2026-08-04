using FluentValidation;

namespace SwiftParcel.Application.Authentication.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("A felhasználónév megadása kötelező.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A jelszó megadása kötelező.");
    }
}