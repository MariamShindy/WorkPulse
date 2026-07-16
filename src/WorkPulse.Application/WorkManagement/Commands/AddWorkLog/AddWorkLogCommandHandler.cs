using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.WorkManagement.Commands.AddWorkLog;

public sealed class AddWorkLogCommandHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser) : IRequestHandler<AddWorkLogCommand, Result<WorkLogDto>>
{
	public async Task<Result<WorkLogDto>> Handle(AddWorkLogCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
		{
			return Error.Unauthorized("Auth.Unauthorized", "Authentication is required.");
		}
		TaskItem task = await context.TaskItems.FirstOrDefaultAsync((TaskItem t) => t.Id == request.TaskId, ct);
		if (task == null)
		{
			return Error.NotFound("Task.NotFound", "Task not found.");
		}
		WorkLog workLog = new WorkLog
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			TaskId = request.TaskId,
			UserId = currentUser.UserId.Value,
			Hours = request.Hours,
			Description = request.Description?.Trim(),
			LoggedDate = request.LoggedDate
		};
		context.WorkLogs.Add(workLog);
		task.LoggedHours += request.Hours;
		return new WorkLogDto(workLog.Id, workLog.TaskId, workLog.UserId, workLog.Hours, workLog.Description, workLog.LoggedDate, workLog.CreatedAtUtc);
	}
}
