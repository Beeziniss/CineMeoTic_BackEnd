using BuildingBlocks.Exceptions;
using CineMeoTic.Common.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models.CQRS;
using CineMeoTic.UserService.API.Services.Intefaces;
using Marten;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class PermissionService(IDocumentSession documentSession) : IPermissionService
{
    private readonly IDocumentSession _documentSession = documentSession;

    private async Task CheckPermissionExistAsync(string permissionName, CancellationToken cancellationToken)
    {
        bool permissionExist = await _documentSession.Query<Permission>().AnyAsync(p => p.Name == permissionName, cancellationToken);
        if (permissionExist)
        {
            throw new BadRequestCustomException(MessageException.PermissionAlreadyExists);
        }
    }
    private async Task CheckPermissionsExistAsync(IEnumerable<string> permissionNames, CancellationToken cancellationToken)
    {
        bool permissionExists = await _documentSession.Query<Permission>().AnyAsync(p => permissionNames.Contains(p.Name), cancellationToken);
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

        _documentSession.Store(permission);
        await _documentSession.SaveChangesAsync(cancellationToken);
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

        _documentSession.Store(permissions.ToArray());
        await _documentSession.SaveChangesAsync(cancellationToken);
    }
}
