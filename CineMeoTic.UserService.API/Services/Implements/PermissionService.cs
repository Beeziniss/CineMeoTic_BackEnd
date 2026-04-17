using BuildingBlocks.Exceptions;
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
        bool permissionExists = await _documentSession.Query<Permission>().AnyAsync(p => p.Name == permissionName, cancellationToken);
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

        documentSession.Store(permission);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
