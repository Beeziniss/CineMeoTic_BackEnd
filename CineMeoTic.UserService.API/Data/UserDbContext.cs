using Microsoft.EntityFrameworkCore;

namespace CineMeoTic.UserService.API.Data;

public sealed class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base (options)
    {
    }

    DbSet<User> Users {get; set;}
    DbSet<Role> Roles {get; set;}
    DbSet<Permission> Permissions {get; set;}
}
