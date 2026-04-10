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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasMany(u => u.Roles)
                                        .WithMany(r => r.Users);

            modelBuilder.Entity<Role>().HasMany(r => r.Permissions)
                                        .WithMany(p => p.Roles);
        }
}
