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

namespace WorkPulse.Application.WorkManagement.Commands.UpdateSprint;

public sealed class UpdateSprintCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<UpdateSprintCommand, Result<SprintDto>>
{
	public async Task<Result<SprintDto>> Handle(UpdateSprintCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Sprint sprint = await context.Sprints.FirstOrDefaultAsync((Sprint s) => s.Id == request.SprintId, ct);
		if (sprint == null)
		{
			return Error.NotFound("Sprint.NotFound", "Sprint not found.");
		}
		if (request.EndDate < request.StartDate)
		{
			return Error.Validation("Sprint.InvalidDateRange", "End date must be on or after start date.");
		}
		sprint.Name = request.Name.Trim();
		sprint.Goal = request.Goal?.Trim();
		sprint.StartDate = request.StartDate;
		sprint.EndDate = request.EndDate;
		sprint.Status = request.Status;
		return new SprintDto(sprint.Id, sprint.TeamId, sprint.Name, sprint.Goal, sprint.StartDate, sprint.EndDate, sprint.Status.ToString(), sprint.CreatedAtUtc);
	}
}
