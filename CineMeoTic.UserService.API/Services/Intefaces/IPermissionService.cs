using CineMeoTic.UserService.API.Models.CQRS;

namespace CineMeoTic.UserService.API.Services.Intefaces;

public interface IPermissionService
{
    Task CreatePermissionAsync(CreatePermissionCommand createPermissionCommand, CancellationToken cancellationToken);
}
