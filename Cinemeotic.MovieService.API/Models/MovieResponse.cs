namespace Cinemeotic.MovieService.API.Models;

public sealed record class MovieResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string Duration { get; init; } = null!;
    public string Country { get; init; } = null!;
    public string Thumbnail { get; init; } = null!;
    public string? Trailer { get; init; }
    public IEnumerable<string> Tags { get; init; } = null!;
    public DateTimeOffset? ReleaseDate { get; init; }
    public IEnumerable<string> Genres { get; init; } = null!;
    public IEnumerable<string> Casters { get; init; } = null!;
    public IEnumerable<string> Directors { get; init; } = null!;
    public double Rating { get; init; } = default;
}
