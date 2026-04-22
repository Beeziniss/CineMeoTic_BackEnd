using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CineMeoTic.UserService.API.Data;

public interface IUserDbContext
{
    public DbSet<User> User { get; }
    public DbSet<Role> Role { get; }
    public DbSet<Permission> Permission { get; }
    public DbSet<UserRole> UserRole { get; }
    public DbSet<RolePermission> RolePermission { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}