using CineMeoTic.UserService.API.Models.Commands;
using CineMeoTic.UserService.API.Services.Intefaces;
using MediatR;

namespace CineMeoTic.UserService.API.CQRS;

public sealed class UserHandler(IUserService userService) : 
    IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserService _userService = userService;

    public async Task<Unit> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(command.UserId, cancellationToken);
        return Unit.Value;
    }
}
