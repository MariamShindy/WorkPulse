
namespace WorkPulse.Application.Abstractions;

public interface IAutomationEngine
{
	Task EvaluateTaskStatusChangeAsync(TaskItem task, Guid previousStateId, CancellationToken ct = default(CancellationToken));
}
