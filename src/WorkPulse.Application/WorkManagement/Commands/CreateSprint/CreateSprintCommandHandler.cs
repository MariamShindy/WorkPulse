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

namespace WorkPulse.Application.WorkManagement.Commands.CreateSprint;

public sealed class CreateSprintCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<CreateSprintCommand, Result<SprintDto>>
{
	public async Task<Result<SprintDto>> Handle(CreateSprintCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		if (!(await context.Teams.AnyAsync((Team t) => t.Id == request.TeamId, ct)))
		{
			return Error.NotFound("Team.NotFound", "Team not found.");
		}
		if (request.EndDate < request.StartDate)
		{
			return Error.Validation("Sprint.InvalidDateRange", "End date must be on or after start date.");
		}
		Sprint sprint = new Sprint
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			TeamId = request.TeamId,
			Name = request.Name.Trim(),
			Goal = request.Goal?.Trim(),
			StartDate = request.StartDate,
			EndDate = request.EndDate,
			Status = request.Status
		};
		context.Sprints.Add(sprint);
		return new SprintDto(sprint.Id, sprint.TeamId, sprint.Name, sprint.Goal, sprint.StartDate, sprint.EndDate, sprint.Status.ToString(), sprint.CreatedAtUtc);
	}
}
