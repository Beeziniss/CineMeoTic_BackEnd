using CineMeoTic.Common.Utils;

namespace CineMeoTic.Common.Models;

public abstract class AuditableTimeStamped
{
    public DateTimeOffset CreatedAt { get; set; } = CustomTimeProvider.GetUtcPlus7TimeOffset();
    public DateTimeOffset? UpdatedAt { get; set; }
}
