
namespace WorkPulse.Application.Analytics.Dtos;

public sealed record AssigneeWorkloadDto(Guid AssigneeId, int OpenTasks, int OverdueTasks, int CompletedTasks);
