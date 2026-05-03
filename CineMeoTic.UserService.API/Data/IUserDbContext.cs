using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CineMeoTic.UserService.API.Data;

public interface IUserDbContext
{
    public DbSet<User> User { get; }
    public DbSet<Role> Role { get; }
    public DbSet<Permission> Permission { get; }
    public DbSet<UserRole> UserRole { get; }
    public DbSet<RolePermission> RolePermission { get; }

    public DatabaseFacade Database { get; }

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken);
}