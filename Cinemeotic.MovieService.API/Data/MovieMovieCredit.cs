namespace Cinemeotic.MovieService.API.Data;

public sealed class MovieMovieCredit
{
    public Guid MovieId { get; set; }
    public Guid MovieCreditId { get; set; }

    public Movie Movie { get; set; } = null!;
    public MovieCredit MovieCredit { get; set; } = null!;
}
