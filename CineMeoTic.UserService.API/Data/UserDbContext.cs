using Microsoft.EntityFrameworkCore;

namespace CineMeoTic.UserService.API.Data;

public partial class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options), IUserDbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Permission> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users);

        modelBuilder.Entity<Role>()
            .HasMany(r => r.Permissions)
            .WithMany(p => p.Roles);
    }
}
