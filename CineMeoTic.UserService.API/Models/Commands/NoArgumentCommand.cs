using BuildingBlocks.CQRS;

namespace CineMeoTic.UserService.API.Models.Commands;

public sealed class NoArgumentCommand : 
    ICommand<LoginCommandResult>
{
}
