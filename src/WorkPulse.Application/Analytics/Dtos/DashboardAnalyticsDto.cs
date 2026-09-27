
namespace WorkPulse.Application.Analytics.Dtos;

public sealed record DashboardAnalyticsDto(int TotalTasks, int OpenTasks, int CompletedTasks, int OverdueTasks, int UnassignedTasks, IReadOnlyList<StatusCountDto> TasksByStatus, IReadOnlyList<PriorityCountDto> TasksByPriority, IReadOnlyList<DailyCountDto> TasksCreatedByDay, IReadOnlyList<DailyCountDto> TasksCompletedByDay, double AverageCycleTimeDays);
