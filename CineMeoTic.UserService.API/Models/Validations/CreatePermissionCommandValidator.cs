using CineMeoTic.Common.Utils;
using CineMeoTic.UserService.API.Models.Commands;
using FluentValidation;

namespace CineMeoTic.UserService.API.Models.Validations;

public sealed class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(MessageException.PermissionRequired)
            .Matches(RegexPattern.PermissionNameRegexPattern).WithMessage(MessageException.InvalidPermissionFormat);
    }
}
