using BuildingBlocks.CQRS;
using FluentValidation;

namespace CineMeoTic.UserService.API.Models.CQRS;

public record LoginCommand(string Email, string Password, bool IsRememberMe = false) : ICommand<LoginCommandResult>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email).NotEmpty().WithMessage("Email is required");

        RuleFor(command => command.Password)
            .NotEmpty().WithMessage("Password is required")
            .Length(3, 150).WithMessage("Password must be between 3 and 150 characters");
    }
}