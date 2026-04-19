using CineMeoTic.Common.Utils;
using CineMeoTic.UserService.API.Models.Commands;
using FluentValidation;

namespace CineMeoTic.UserService.API.Models.Validations;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(MessageException.EmailRequired)
            .EmailAddress().WithMessage(MessageException.InvalidEmailFormat);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(MessageException.PasswordRequired)
            .Matches(RegexPattern.PasswordRegexPattern).WithMessage(MessageException.InvalidPasswordFormat);
    }
}