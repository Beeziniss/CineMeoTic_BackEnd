namespace BuildingBlocks.Models;

public abstract class Auditable : AuditableTimeStamped
{
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
