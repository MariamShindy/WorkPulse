using System;
using System.Threading;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using WorkPulse.Infrastructure.Jobs;

namespace WorkPulse.Infrastructure.Extensions;

public static class HangfireJobExtensions
{
	public static void ScheduleRecurringJobs(this IServiceProvider services)
	{
		using IServiceScope serviceScope = services.CreateScope();
		IRecurringJobManager requiredService = serviceScope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
		requiredService.AddOrUpdate("outbox-processor", (OutboxProcessorJob job) => job.ProcessAsync(CancellationToken.None), Cron.Minutely);
		requiredService.AddOrUpdate("deadline-reminder", (DeadlineReminderJob job) => job.RunAsync(CancellationToken.None), Cron.Daily(8));
		requiredService.AddOrUpdate("sla-monitor", (SlaMonitorJob job) => job.RunAsync(CancellationToken.None), Cron.Hourly);
	}
}
