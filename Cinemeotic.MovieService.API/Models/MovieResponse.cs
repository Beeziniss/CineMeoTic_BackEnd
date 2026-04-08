namespace Cinemeotic.MovieService.API.Models;

public sealed record class MovieResponse
{
    public Guid Id { get; }
    public string Title { get; } = null!;
    public string Description { get; } = null!;
    public string Duration { get; } = null!;
    public string Country { get; } = null!;
    public string Thumbnail { get; } = null!;
    public string? Trailer { get; }
    public IEnumerable<string> Tags { get; } = null!;
    public DateTimeOffset? ReleaseDate { get; }
    public IEnumerable<string> Genres { get; } = null!;
    public IEnumerable<string> Casters { get; } = null!;
    public IEnumerable<string> Directors { get; } = null!;
    public double Rating { get; } = default;
}
