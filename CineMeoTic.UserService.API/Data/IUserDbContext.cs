using Microsoft.EntityFrameworkCore;

namespace CineMeoTic.UserService.API.Data
{
    public interface IUserDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<Permission> Permissions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}