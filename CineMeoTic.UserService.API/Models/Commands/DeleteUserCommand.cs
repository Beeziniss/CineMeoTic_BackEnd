using BuildingBlocks.CQRS;

namespace CineMeoTic.UserService.API.Models.Commands;

public sealed record DeleteUserCommand() : ICommand
{
    public Guid UserId { get; init; }
};