using BuildingBlocks.Exceptions;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models.CQRS;
using CineMeoTic.UserService.API.Services.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class RoleService(IUserDbContext userDbContext) : IRoleService
{
    private readonly IUserDbContext _userDbContext = userDbContext;

    private async Task CheckRoleExistAsync(string roleName)
    {
        bool roleExists = await _userDbContext.Role.AnyAsync(r => r.Name == roleName);
        if (roleExists)
        {
            throw new BadRequestCustomException($"Role '{roleName}' already exists.");
        }
    }
    private async Task CheckPermissionExistAsync(IEnumerable<string> permissionNames)
    {
        foreach (string permissionName in permissionNames)
        {
            bool permissionExists = await _userDbContext.Permission.AnyAsync(p => p.Name == permissionName);
            if (!permissionExists)
            {
                throw new BadRequestCustomException($"Permission '{permissionName}' does not exist.");
            }
        }
    }
    public async Task CreateRoleAsync(CreateRoleCommand roleCommand, CancellationToken cancellationToken)
    {
        await CheckRoleExistAsync(roleCommand.RoleName);
        await CheckPermissionExistAsync(roleCommand.PermissionNames);

        Role role = new()
        {
            Name = roleCommand.RoleName,
        };
        IReadOnlyCollection<Permission> permissions = await _userDbContext.Permission
            .Where(p => roleCommand.PermissionNames.Contains(p.Name))
            .ToListAsync();
        permissions.ToList().ForEach(x => role.Permissions.Add(x));

        _userDbContext.Role.Add(role);
        await _userDbContext.SaveChangesAsync(cancellationToken);
    }
}
