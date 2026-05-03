using BuildingBlocks.Exceptions;
using BuildingBlocks.Utils;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models.Commands;
using CineMeoTic.UserService.API.Services.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class RoleService(IUserDbContext userDbContext) : IRoleService
{
    private readonly IUserDbContext _userDbContext = userDbContext;

    private static async Task CheckRoleExistAsync(IUserDbContext userDbContext, string roleName, CancellationToken cancellationToken)
    {
        bool roleExists = await userDbContext.Role.AsNoTracking().AnyAsync(r => r.Name == roleName, cancellationToken);
        if (roleExists)
        {
            throw new BadRequestCustomException(MessageException.RoleAlreadyExists);
        }
    }
    public async Task CreateRoleAsync(CreateRoleCommand roleCommand, CancellationToken cancellationToken)
    {
        await CheckRoleExistAsync(_userDbContext, roleCommand.RoleName, cancellationToken);

        Role role = new()
        {
            Id = Guid.NewGuid(),
            Name = roleCommand.RoleName,
        };

        IReadOnlyCollection<Permission> permissions = await _userDbContext.Permission
            .AsNoTracking()
            .Where(p => roleCommand.PermissionNames.Contains(p.Name))
            .ToListAsync(cancellationToken);

        IEnumerable<RolePermission> rolePermissions = permissions
            .Select(permission => new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id,
            })
            .ToList();

        _userDbContext.Role.Add(role);
        _userDbContext.RolePermission.AddRange(rolePermissions);
    }
}
