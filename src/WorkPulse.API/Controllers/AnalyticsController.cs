using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Analytics.Queries.GetAssigneeWorkload;
using WorkPulse.Application.Analytics.Queries.GetDashboardAnalytics;
using WorkPulse.Application.Analytics.Queries.GetProjectProgress;
using WorkPulse.Application.Analytics.Queries.GetTeamVelocity;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class AnalyticsController(ISender sender) : ControllerBase
{
	[HttpGet("dashboard")]
	public async Task<ActionResult<DashboardAnalyticsDto>> Dashboard([FromQuery] Guid? teamId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<DashboardAnalyticsDto>>)new GetDashboardAnalyticsQuery(teamId, from, to), ct)).ToActionResult();
	}

	[HttpGet("velocity")]
	public async Task<ActionResult<IReadOnlyList<TeamVelocityPointDto>>> Velocity([FromQuery] Guid? teamId, [FromQuery] int weeks = 12, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<IReadOnlyList<TeamVelocityPointDto>>>)new GetTeamVelocityQuery(teamId, weeks), ct)).ToActionResult();
	}

	[HttpGet("workload")]
	public async Task<ActionResult<IReadOnlyList<AssigneeWorkloadDto>>> Workload([FromQuery] Guid? teamId, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<IReadOnlyList<AssigneeWorkloadDto>>>)new GetAssigneeWorkloadQuery(teamId), ct)).ToActionResult();
	}

	[HttpGet("project-progress")]
	public async Task<ActionResult<IReadOnlyList<ProjectProgressDto>>> ProjectProgress([FromQuery] Guid? teamId, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<IReadOnlyList<ProjectProgressDto>>>)new GetProjectProgressQuery(teamId), ct)).ToActionResult();
	}
}
