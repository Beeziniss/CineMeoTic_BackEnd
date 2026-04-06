using CineMeoTic.Common.Models;

namespace Cinemeotic.MovieService.API.Data;

public sealed class Genre : AuditableTimeStamped
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Movie> Movies { get; set; } = null!;
}
