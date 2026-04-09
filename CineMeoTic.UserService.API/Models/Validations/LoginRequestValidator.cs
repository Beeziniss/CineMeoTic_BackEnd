using CineMeoTic.Common.Utils;
using FluentValidation;

namespace CineMeoTic.UserService.API.Models.Validations;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Matches(RegexPattern.PasswordRegexPattern).WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and be at least 6 characters long.");
    }
}
