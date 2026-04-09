namespace Cinemeotic.MovieService.API.Models;

public sealed record class GenreResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Slug { get; init; } = null!;
    public IEnumerable<string> Aliases { get; init; } = null!;
}
