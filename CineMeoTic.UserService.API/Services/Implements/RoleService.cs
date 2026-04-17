using BuildingBlocks.Exceptions;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models.CQRS;
using CineMeoTic.UserService.API.Services.Intefaces;
using Marten;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class RoleService(IDocumentStore documentStore) : IRoleService
{
    private readonly IDocumentStore _documentStore = documentStore;

    private static async Task CheckRoleExistAsync(IDocumentSession documentSession, string roleName, CancellationToken cancellationToken)
    {
        bool roleExists = await documentSession.Query<Role>().AnyAsync(r => r.Name == roleName, cancellationToken);
        if (roleExists)
        {
            throw new BadRequestCustomException($"Role '{roleName}' already exists.");
        }
    }

    private static async Task CheckPermissionExistAsync(IDocumentSession documentSession, IEnumerable<string> permissionNames, CancellationToken cancellationToken)
    {
        foreach (string permissionName in permissionNames)
        {
            bool permissionExists = await documentSession.Query<Permission>().AnyAsync(p => p.Name == permissionName, cancellationToken);
            if (!permissionExists)
            {
                throw new BadRequestCustomException($"Permission '{permissionName}' does not exist.");
            }
        }
    }

    public async Task CreateRoleAsync(CreateRoleCommand roleCommand, CancellationToken cancellationToken)
    {
        using IDocumentSession documentSession = _documentStore.LightweightSession();

        await CheckRoleExistAsync(documentSession, roleCommand.RoleName, cancellationToken);
        await CheckPermissionExistAsync(documentSession, roleCommand.PermissionNames, cancellationToken);

        Role role = new()
        {
            Name = roleCommand.RoleName,
        };

        IReadOnlyCollection<Permission> permissions = await documentSession.Query<Permission>()
            .Where(p => roleCommand.PermissionNames.Contains(p.Name))
            .ToListAsync(cancellationToken);

        documentSession.Store(role);

        RolePermission[] rolePermissions = permissions
            .Select(permission => new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id,
            })
            .ToArray();

        if (rolePermissions.Length > 0)
        {
            documentSession.Store(rolePermissions);
        }

        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
