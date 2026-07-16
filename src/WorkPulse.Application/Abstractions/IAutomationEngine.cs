using System;
using System.Threading;
using System.Threading.Tasks;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Abstractions;

public interface IAutomationEngine
{
	Task EvaluateTaskStatusChangeAsync(TaskItem task, Guid previousStateId, CancellationToken ct = default(CancellationToken));
}
