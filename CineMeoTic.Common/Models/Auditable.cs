namespace CineMeoTic.Common.Models;

public abstract class Auditable : TimeStamped
{
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
