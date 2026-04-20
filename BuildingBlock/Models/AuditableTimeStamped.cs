using BuildingBlocks.Utils;

namespace BuildingBlocks.Models;

public abstract class AuditableTimeStamped
{
    public DateTimeOffset CreatedAt { get; set; } = CustomTimeProvider.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
