using Microsoft.AspNetCore.Authorization;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Projects.Commands.ArchiveProject;
using WorkPulse.Application.Projects.Commands.CreateProject;
using WorkPulse.Application.Projects.Commands.UpdateProject;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Application.Projects.Queries.GetProject;
using WorkPulse.Application.Projects.Queries.ListProjects;
using WorkPulse.Domain.Enums;
using WorkPulse.API.Contracts.Projects;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class ProjectsController(ISender sender) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<PagedList<ProjectDto>>> List([FromQuery] Guid? teamId, [FromQuery] ProjectStatus? status, [FromQuery] bool includeArchived = false, [FromQuery] string? sortBy = null, [FromQuery] string sortDirection = "asc", [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<ProjectDto>>>)new ListProjectsQuery(new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}, new SortParams
		{
			SortBy = sortBy,
			SortDirection = sortDirection
		}, teamId, status, includeArchived), ct)).ToActionResult();
	}

	[HttpGet("{projectId:guid}")]
	public async Task<ActionResult<ProjectDto>> Get(Guid projectId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<ProjectDto>>)new GetProjectQuery(projectId), ct)).ToActionResult();
	}

	[HttpPost]
	public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectRequest request, CancellationToken ct)
	{
		Result<ProjectDto> result = await sender.Send((IRequest<Result<ProjectDto>>)new CreateProjectCommand(request.TeamId, request.Name, request.Key, request.Description, request.Status, request.LeadId, request.StartDate, request.TargetDate), ct);
		if (result.IsFailure)
		{
			return result.ToActionResult();
		}
		return CreatedAtAction("Get", new
		{
			projectId = result.Value.Id
		}, result.Value);
	}

	[HttpPut("{projectId:guid}")]
	public async Task<ActionResult<ProjectDto>> Update(Guid projectId, [FromBody] UpdateProjectRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<ProjectDto>>)new UpdateProjectCommand(projectId, request.Name, request.Description, request.Status, request.LeadId, request.StartDate, request.TargetDate), ct)).ToActionResult();
	}

	[HttpPost("{projectId:guid}/archive")]
	public async Task<IActionResult> Archive(Guid projectId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new ArchiveProjectCommand(projectId), ct)).ToActionResult();
	}
}
