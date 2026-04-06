using CineMeoTic.Common.Models;

namespace Cinemeotic.MovieService.API.Data;

public sealed class Review : AuditableTimeStamped
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public double Rating { get; set; }
    public string Comment { get; set; } = null!;
}
