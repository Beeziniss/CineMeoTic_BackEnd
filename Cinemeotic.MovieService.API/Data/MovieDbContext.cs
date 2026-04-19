using Microsoft.EntityFrameworkCore;

namespace Cinemeotic.MovieService.API.Data;

public class MovieDbContext(DbContextOptions<MovieDbContext> options) : DbContext(options)
{
    public DbSet<Movie> Movie { get; set; }
    public DbSet<Genre> Genre { get; set; }
    public DbSet<MovieCredit> MovieCredit { get; set; }
    public DbSet<Comment> Comment { get; set; }
    public DbSet<MovieRating> MovieRating { get; set; }
    public DbSet<MovieGenre> MovieGenre { get; set; }
    public DbSet<MovieMovieCredit> MovieMovieCredit { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>().HasMany(m => m.MovieCredits)
                                    .WithMany(mc => mc.Movies)
                                    .UsingEntity<MovieMovieCredit>(
                                        m => m.HasOne(mmc => mmc.MovieCredit)
                                              .WithMany()
                                              .HasForeignKey(mmc => mmc.MovieCreditId)
                                              .OnDelete(DeleteBehavior.Cascade),
                                        m => m.HasOne(mmc => mmc.Movie)
                                              .WithMany()
                                              .HasForeignKey(mmc => mmc.MovieId)
                                              .OnDelete(DeleteBehavior.Cascade),
                                        m =>
                                        {
                                            m.ToTable("MovieMovieCredits");
                                            m.HasKey(mmc => new { mmc.MovieId, mmc.MovieCreditId });
                                        });

        modelBuilder.Entity<Movie>().HasMany(m => m.Comments)
                                    .WithOne(c => c.Movie)
                                    .HasForeignKey(c => c.MovieId)
                                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Movie>().HasMany(m => m.Genres)
                                    .WithMany(c => c.Movies)
                                    .UsingEntity<MovieGenre>(
                                        m => m.HasOne(mg => mg.Genre)
                                              .WithMany()
                                              .HasForeignKey(mg => mg.GenreId)
                                              .OnDelete(DeleteBehavior.Cascade),
                                        m => m.HasOne(mg => mg.Movie)
                                              .WithMany()
                                              .HasForeignKey(mg => mg.MovieId)
                                              .OnDelete(DeleteBehavior.Cascade),
                                        m =>
                                        {
                                            m.ToTable("MovieGenres");
                                            m.HasKey(mg => new { mg.MovieId, mg.GenreId });
                                        });

        modelBuilder.Entity<Movie>().HasMany(m => m.MovieRatings)
                                    .WithOne(r => r.Movie)
                                    .HasForeignKey(r => r.MovieId)
                                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieRating>()
                    .HasIndex(r => new { r.MovieId, r.UserId })
                    .IsUnique();
    }
}