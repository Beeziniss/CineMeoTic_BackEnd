using Microsoft.EntityFrameworkCore;

namespace Cinemeotic.MovieService.API.Data
{
    public sealed class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
        {
        }
        
        DbSet<Movie> Movies {get; set;}
        DbSet<Genre> Genres {get; set;}
        DbSet<MovieCredit> MovieCredits {get; set;}
        DbSet<Comment> Comments {get; set;}
        DbSet<MovieRating> MovieRatings {get; set;}
    }
}