using BuildingBlocks.Models;

namespace Cinemeotic.MovieService.API.Data;

public sealed class Movie : AuditableTimeStamped
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string UnsignedTitle { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public int? Duration { get; set; }
    public string Country { get; set; } = null!;
    public string Thumbnail { get; set; } = null!;
    public string? Trailer { get; set; }
    public List<string> Tags { get; set; } = []; // JSON
    public DateTimeOffset? ReleaseDate { get; set; }

    public ICollection<MovieCredit> MovieCredits { get; } = [];
    public ICollection<Genre> Genres { get; } = [];
    public ICollection<MovieRating> MovieRatings { get; } = [];
    public ICollection<Comment> Comments { get; } = [];
}
