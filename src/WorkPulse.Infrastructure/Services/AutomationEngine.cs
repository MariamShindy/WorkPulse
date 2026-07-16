using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Collaboration.Services;
using WorkPulse.Domain.Entities;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Infrastructure.Services;

public sealed class AutomationEngine(IApplicationDbContext context, ITaskCollaborationService collaboration) : IAutomationEngine
{
	public async Task EvaluateTaskStatusChangeAsync(TaskItem task, Guid previousStateId, CancellationToken ct = default(CancellationToken))
	{
		foreach (AutomationRule rule in await (from r in context.AutomationRules.AsNoTracking()
			where r.IsEnabled && (int)r.TriggerType == 0
			select r).ToListAsync(ct))
		{
			if (MatchesStatusTrigger(rule.TriggerConfigJson, previousStateId, task.WorkflowStateId))
			{
				await ExecuteActionAsync(rule, task, ct);
			}
		}
	}

	private static bool MatchesStatusTrigger(string configJson, Guid previousStateId, Guid newStateId)
	{
		try
		{
			using JsonDocument jsonDocument = JsonDocument.Parse(configJson);
			JsonElement rootElement = jsonDocument.RootElement;
			if (rootElement.TryGetProperty("fromStateId", out var value) && value.GetGuid() != previousStateId)
			{
				return false;
			}
			if (rootElement.TryGetProperty("toStateId", out var value2) && value2.GetGuid() != newStateId)
			{
				return false;
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	private async Task ExecuteActionAsync(AutomationRule rule, TaskItem task, CancellationToken ct)
	{
		using JsonDocument doc = JsonDocument.Parse(rule.ActionConfigJson);
		JsonElement root = doc.RootElement;
		switch (rule.ActionType)
		{
		case AutomationActionType.SendNotification:
		{
			JsonElement titleEl;
			string title = (root.TryGetProperty("title", out titleEl) ? (titleEl.GetString() ?? "Automation triggered") : "Automation triggered");
			JsonElement bodyEl;
			string body = (root.TryGetProperty("body", out bodyEl) ? (bodyEl.GetString() ?? string.Empty) : string.Empty);
			if (task.AssigneeId.HasValue)
			{
				await collaboration.NotifyUsersAsync(task.TenantId, [task.AssigneeId.Value], NotificationType.TaskStatusChanged, title, body, null, "TaskItem", task.Id, ct);
			}
			break;
		}
		case AutomationActionType.UpdatePriority:
		{
			if (root.TryGetProperty("priority", out var priorityEl) && Enum.TryParse<TaskPriority>(priorityEl.GetString(), ignoreCase: true, out var priority))
			{
				TaskItem tracked2 = await context.TaskItems.FirstOrDefaultAsync((TaskItem t) => t.Id == task.Id, ct);
				if (tracked2 != null)
				{
					tracked2.Priority = priority;
				}
			}
			break;
		}
		case AutomationActionType.AssignUser:
		{
			if (root.TryGetProperty("userId", out var userEl))
			{
				TaskItem tracked = await context.TaskItems.FirstOrDefaultAsync((TaskItem t) => t.Id == task.Id, ct);
				if (tracked != null)
				{
					tracked.AssigneeId = userEl.GetGuid();
				}
			}
			break;
		}
		}
	}
}
