using CineMeoTic.Common.Utils;
using FluentValidation;

namespace CineMeoTic.UserService.API.Models.Validations;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Matches(RegexPattern.PasswordRegexPattern).WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and be at least 6 characters long.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("ConfirmPassword is required.")
            .Equal(x => x.Password).WithMessage("ConfirmPassword must match Password.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("DisplayName is required.")
            .MinimumLength(2).WithMessage("DisplayName must be at least 2 characters long.")
            .MaximumLength(100).WithMessage("DisplayName must be at most 100 characters long.");
    }
}
