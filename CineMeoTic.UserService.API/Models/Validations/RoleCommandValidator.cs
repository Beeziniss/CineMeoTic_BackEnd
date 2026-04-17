using CineMeoTic.UserService.API.Models.CQRS;
using FluentValidation;

namespace CineMeoTic.UserService.API.Models.Validations;

public sealed class RoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public RoleCommandValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.");

        RuleFor(x => x.PermissionNames)
            .NotEmpty().WithMessage("At least one permission is required.");
    }
}
