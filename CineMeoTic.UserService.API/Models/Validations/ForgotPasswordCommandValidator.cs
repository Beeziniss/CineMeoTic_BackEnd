using BuildingBlocks.Utils;
using CineMeoTic.UserService.API.Models.Commands;
using FluentValidation;

namespace CineMeoTic.UserService.API.Models.Validations;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(MessageException.EmailRequired)
            .EmailAddress().WithMessage(MessageException.InvalidEmailFormat);
    }
}
