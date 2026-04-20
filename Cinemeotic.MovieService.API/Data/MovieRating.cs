using BuildingBlocks.Models;

namespace Cinemeotic.MovieService.API.Data;

public sealed class MovieRating : AuditableTimeStamped
{
    public Guid Id { get; set; }
    public Guid MovieId { get; set; }
    public Guid UserId { get; set; }
    public double Rating { get; set; }

    public Movie? Movie { get; set; }
}
