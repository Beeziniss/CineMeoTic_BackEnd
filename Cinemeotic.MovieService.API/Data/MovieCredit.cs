using BuildingBlocks.Models;
using Cinemeotic.MovieService.API.Data.Enums;

namespace Cinemeotic.MovieService.API.Data;

public sealed class MovieCredit : AuditableTimeStamped
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Biography { get; set; }
    public string? Cover { get; set; }
    public Role Role { get; set; }
    public ICollection<Movie>? Movies { get;}
}
