using CineMeoTic.UserService.API.Models.CQRS;
using FluentValidation;

namespace CineMeoTic.UserService.API.Models.Validations;

public sealed class PermissionCommandValidation : AbstractValidator<CreatePermissionCommand>
{
    public PermissionCommandValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
    }
}