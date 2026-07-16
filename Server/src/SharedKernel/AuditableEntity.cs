namespace SharedKernel;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedOnUtc { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? DeletedOnUtc { get; set; }
    public bool IsDeleted { get; set; }
}
