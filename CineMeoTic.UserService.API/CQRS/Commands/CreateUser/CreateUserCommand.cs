using BuildingBlocks.CQRS;
using CineMeoTic.UserService.API.Models;
using FluentValidation;

namespace CineMeoTic.UserService.API.CQRS.Commands.CreateUser
{
    public record CreateUserCommand (CreateUserRequest UserModel): ICommand<CreateUserResponse>;

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(u => u.UserModel.Username).NotEmpty().WithMessage("Username is required");
            RuleFor(u => u.UserModel.Password).NotEmpty().WithMessage("Password is required");
        }
    }
}