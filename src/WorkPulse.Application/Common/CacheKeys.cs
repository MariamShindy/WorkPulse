
namespace WorkPulse.Application.Common;

public static class CacheKeys
{
	public static string Dashboard(Guid tenantId, Guid? teamId, DateOnly? from, DateOnly? to)
	{
		return $"analytics:dashboard:{tenantId}:{teamId}:{from}:{to}";
	}

	public static string TeamVelocity(Guid tenantId, Guid? teamId, int weeks)
	{
		return $"analytics:velocity:{tenantId}:{teamId}:{weeks}";
	}

	public static string AssigneeWorkload(Guid tenantId, Guid? teamId)
	{
		return $"analytics:workload:{tenantId}:{teamId}";
	}

	public static string ProjectProgress(Guid tenantId, Guid? teamId)
	{
		return $"analytics:progress:{tenantId}:{teamId}";
	}

	public static string AnalyticsPrefix(Guid tenantId)
	{
		return $"analytics:{tenantId}:";
	}

	public static string CycleTimeAnalytics(Guid tenantId, Guid? teamId, DateOnly? from, DateOnly? to)
	{
		return $"analytics:cycletime:{tenantId}:{teamId}:{from}:{to}";
	}

	public static string Throughput(Guid tenantId, Guid? teamId, string granularity, int periods)
	{
		return $"analytics:throughput:{tenantId}:{teamId}:{granularity}:{periods}";
	}

	public static string SprintBurndown(Guid tenantId, Guid sprintId)
	{
		return $"analytics:burndown:{tenantId}:{sprintId}";
	}
}
