using Microsoft.EntityFrameworkCore;

namespace CineMeoTic.UserService.API.Data;

public partial class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options), IUserDbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Permission> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity("UserRoles");

        modelBuilder.Entity<Role>()
            .HasMany(r => r.Permissions)
            .WithMany(p => p.Roles)
            .UsingEntity("RolePermissions");
    }
}
