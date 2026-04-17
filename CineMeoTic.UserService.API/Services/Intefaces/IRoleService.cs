using CineMeoTic.UserService.API.Models.CQRS;

namespace CineMeoTic.UserService.API.Services.Intefaces;

public interface IRoleService
{
    Task CreateRoleAsync(CreateRoleCommand roleCommand, CancellationToken cancellationToken);
}
