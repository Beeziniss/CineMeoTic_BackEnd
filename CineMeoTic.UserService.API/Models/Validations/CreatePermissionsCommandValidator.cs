using CineMeoTic.Common.Utils;
using CineMeoTic.UserService.API.Models.Commands;
using FluentValidation;

namespace CineMeoTic.UserService.API.Models.Validations;

public sealed class CreatePermissionsCommandValidator : AbstractValidator<CreatePermissionsCommand>
{
    public CreatePermissionsCommandValidator()
    {
        RuleFor(x => x.Names)
            .NotEmpty().WithMessage(MessageException.NameRequired);

        RuleForEach(x => x.Names)
            .Matches(RegexPattern.PermissionNameRegexPattern).WithMessage(MessageException.InvalidPermissionFormat);
    }
}
