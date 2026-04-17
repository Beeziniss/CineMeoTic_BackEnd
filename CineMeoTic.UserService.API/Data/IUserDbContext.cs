using Microsoft.EntityFrameworkCore;

namespace CineMeoTic.UserService.API.Data
{
    public interface IUserDbContext
    {
        DbSet<User> User { get; }
        DbSet<Role> Role { get; }
        DbSet<Permission> Permission { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}