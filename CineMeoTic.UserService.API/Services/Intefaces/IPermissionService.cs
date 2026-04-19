using CineMeoTic.UserService.API.Models.Commands;

namespace CineMeoTic.UserService.API.Services.Intefaces;

public interface IPermissionService
{
    Task CreatePermissionAsync(CreatePermissionCommand createPermissionCommand, CancellationToken cancellationToken);
    Task CreatePermissionsAsync(CreatePermissionsCommand createPermissionsCommand, CancellationToken cancellationToken);
}
