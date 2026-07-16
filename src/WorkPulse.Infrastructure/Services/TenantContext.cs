using System;
using WorkPulse.Application.Abstractions;

namespace WorkPulse.Infrastructure.Services;

public sealed class TenantContext : ITenantContext
{
	public Guid TenantId { get; private set; }

	public string? TenantSlug { get; private set; }

	public bool IsResolved { get; private set; }

	public void Set(Guid tenantId, string? slug = null)
	{
		TenantId = tenantId;
		TenantSlug = slug;
		IsResolved = true;
	}
}
