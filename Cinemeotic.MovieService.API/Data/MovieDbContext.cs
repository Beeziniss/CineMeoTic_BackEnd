using Microsoft.EntityFrameworkCore;

namespace Cinemeotic.MovieService.API.Data;

public class MovieDbContext : DbContext
{
    public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
    {
    }
    
    DbSet<Movie> Movies {get; set;}
    DbSet<Genre> Genres {get; set;}
    DbSet<MovieCredit> MovieCredits {get; set;}
    DbSet<Comment> Comments {get; set;}
    DbSet<MovieRating> MovieRatings {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>().HasMany(m => m.MovieCredits)
                                    .WithMany(mc => mc.Movies);

        modelBuilder.Entity<Movie>().HasMany(m => m.Comments)
                                    .WithOne(c => c.Movie)
                                    .HasForeignKey(c => c.MovieId)
                                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Movie>().HasMany(m => m.Genres)
                                    .WithMany(c => c.Movies);

        modelBuilder.Entity<Movie>().HasMany(m => m.MovieRatings)
                                    .WithOne(r => r.Movie)
                                    .HasForeignKey(r => r.MovieId)
                                    .OnDelete(DeleteBehavior.Cascade);
    }
}