using WorkPulse.Domain.Common;

namespace WorkPulse.Domain.Entities;

public sealed class TaskLabel : TenantEntity
{
	public Guid TaskId { get; set; }

	public Guid LabelId { get; set; }
}
