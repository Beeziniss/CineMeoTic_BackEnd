using BuildingBlocks.Utils;
using CineMeoTic.UserService.API.Models.Commands;
using FluentValidation;

namespace CineMeoTic.UserService.API.Models.Validations;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Otp)
            .NotEmpty().WithMessage(MessageException.InvalidOtp);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(MessageException.EmailRequired)
            .EmailAddress().WithMessage(MessageException.InvalidEmailFormat);

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage(MessageException.NewPasswordRequired)
            .Matches(RegexPattern.PasswordRegexPattern).WithMessage(MessageException.InvalidPasswordFormat);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage(MessageException.ConfirmPasswordRequired)
            .Equal(x => x.NewPassword).WithMessage(MessageException.ConfirmPasswordMustMatchNewPassword);
    }
}
