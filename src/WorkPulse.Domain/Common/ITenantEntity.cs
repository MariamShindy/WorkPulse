using System;

namespace WorkPulse.Domain.Common;

public interface ITenantEntity
{
	Guid TenantId { get; }
}
