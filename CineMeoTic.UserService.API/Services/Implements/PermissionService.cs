using BuildingBlocks.Exceptions;
using BuildingBlocks.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models.Commands;
using CineMeoTic.UserService.API.Services.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class PermissionService(IUserDbContext userDbContext) : IPermissionService
{
    private readonly IUserDbContext _userDbContext = userDbContext;

    private async Task CheckPermissionExistAsync(string permissionName, CancellationToken cancellationToken)
    {
        bool permissionExist = await _userDbContext.Permission.AnyAsync(p => p.Name == permissionName, cancellationToken);
        if (permissionExist)
        {
            throw new BadRequestCustomException(MessageException.PermissionAlreadyExists);
        }
    }
    private async Task CheckPermissionsExistAsync(IEnumerable<string> permissionNames, CancellationToken cancellationToken)
    {
        bool permissionExists = await _userDbContext.Permission.AnyAsync(p => permissionNames.Contains(p.Name), cancellationToken);
        if (permissionExists)
        {
            throw new BadRequestCustomException(MessageException.PermissionsAlreadyExist);
        }
    }
    public async Task CreatePermissionAsync(CreatePermissionCommand createPermissionCommand, CancellationToken cancellationToken)
    {
        await CheckPermissionExistAsync(createPermissionCommand.Name, cancellationToken);

        Permission permission = new()
        {
            Name = createPermissionCommand.Name
        };

        _userDbContext.Permission.Add(permission);
    }
    public async Task CreatePermissionsAsync(CreatePermissionsCommand createPermissionsCommand, CancellationToken cancellationToken)
    {
        await CheckPermissionsExistAsync(createPermissionsCommand.Names, cancellationToken);

        List<Permission> permissions = [];
        foreach (string name in createPermissionsCommand.Names)
        {
            Permission permission = new()
            {
                Name = name
            };

            permissions.Add(permission);
        }

        _userDbContext.Permission.AddRange(permissions);
    }
}
