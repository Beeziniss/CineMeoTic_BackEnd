using BuildingBlocks.Exceptions;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models.CQRS;
using CineMeoTic.UserService.API.Services.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class PermissionService(IUserDbContext userDbContext) : IPermissionService
{
    private readonly IUserDbContext _userDbContext = userDbContext;

    private async Task CheckPermissionExistAsync(string permissionName, CancellationToken cancellationToken)
    {
        bool permissionExists = await _userDbContext.Permission.AnyAsync(p => p.Name == permissionName, cancellationToken);
        if (permissionExists)
        {
            throw new BadRequestCustomException($"Permission '{permissionName}' already exists.");
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
        await _userDbContext.SaveChangesAsync(cancellationToken);
    }
}
