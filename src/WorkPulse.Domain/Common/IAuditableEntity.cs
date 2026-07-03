namespace WorkPulse.Domain.Common;

public interface IAuditableEntity
{
    DateTime CreatedAtUtc { get; }
    Guid? CreatedById { get; }
    DateTime? UpdatedAtUtc { get; }
    Guid? UpdatedById { get; }
}
