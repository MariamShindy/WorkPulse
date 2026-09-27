using WorkPulse.Domain.Common;

namespace WorkPulse.Application.Common.Extensions;

public static class TenantQueryExtensions
{
	public static IQueryable<T> ForTenant<T>(this IQueryable<T> query, ITenantContext tenantContext)
		where T : class, ITenantEntity
		=> query.Where((T e) => e.TenantId == tenantContext.TenantId);
}
