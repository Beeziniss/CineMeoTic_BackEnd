using BuildingBlocks.Exceptions;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models.CQRS;
using CineMeoTic.UserService.API.Services.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class PermissionService(IUserDbContext userDbContext) : IPermissionService
{
    private readonly IUserDbContext _userDbContext = userDbContext;

    private async Task CheckPermissionExistAsync(string permissionName)
    {
        bool permissionExists = await _userDbContext.Permissions.AnyAsync(p => p.Name == permissionName);
        if (permissionExists)
        {
            throw new BadRequestCustomException($"Permission '{permissionName}' already exists.");
        }
    }
    public async Task CreatePermissionAsync(CreatePermissionCommand createPermissionCommand, CancellationToken cancellationToken)
    {
        await CheckPermissionExistAsync(createPermissionCommand.Name);

        Permission permission = new()
        {
            Name = createPermissionCommand.Name
        };

        _userDbContext.Permissions.Add(permission);
        await _userDbContext.SaveChangesAsync(cancellationToken);
    }
}
