using CineMeoTic.Common.Utils;
using CineMeoTic.UserService.API.Models.CQRS;
using FluentValidation;

namespace CineMeoTic.UserService.API.Models.Validations;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(MessageException.PasswordRequired)
            .Matches(RegexPattern.PasswordRegexPattern).WithMessage(MessageException.InvalidPasswordFormat);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("ConfirmPassword is required.")
            .Equal(x => x.Password).WithMessage("ConfirmPassword must match Password.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage(MessageException.DisplayNameRequired)
            .MinimumLength(2).WithMessage(MessageException.DisplayMinNameLength)
            .MaximumLength(100).WithMessage(MessageException.DisplayMaxNameLength);
    }
}
