using BuildingBlocks.Utils;
using CineMeoTic.UserService.API.Models.Commands;
using FluentValidation;

namespace CineMeoTic.UserService.API.Models.Validations;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage(MessageException.CurrentPasswordRequired);

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage(MessageException.NewPasswordRequired)
            .Matches(RegexPattern.PasswordRegexPattern).WithMessage(MessageException.InvalidPasswordFormat);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage(MessageException.ConfirmPasswordRequired)
            .Equal(x => x.NewPassword).WithMessage(MessageException.ConfirmPasswordMustMatchNewPassword);
    }
}
