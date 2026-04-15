using CineMeoTic.Common.Utils;
using CineMeoTic.UserService.API.Models.CQRS;
using FluentValidation;

namespace CineMeoTic.UserService.API.Models.Validations;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(MessageException.EmailRequired)
            .EmailAddress().WithMessage(MessageException.EmailInvalidFormat);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(MessageException.PasswordRequired)
            .Matches(RegexPattern.PasswordRegexPattern).WithMessage(MessageException.PasswordInvalidFormat);
    }
}