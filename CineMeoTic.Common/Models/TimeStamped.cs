namespace CineMeoTic.Common.Models;

public abstract class TimeStamped
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
