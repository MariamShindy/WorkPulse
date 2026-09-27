using Microsoft.AspNetCore.Authorization;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Organizations.Commands.AddTeamMember;
using WorkPulse.Application.Organizations.Commands.ArchiveTeam;
using WorkPulse.Application.Organizations.Commands.CreateTeam;
using WorkPulse.Application.Organizations.Commands.RemoveTeamMember;
using WorkPulse.Application.Organizations.Commands.UpdateTeam;
using WorkPulse.Application.Organizations.Commands.UpdateTeamMember;
using WorkPulse.Application.Organizations.Dtos;
using WorkPulse.Application.Organizations.Queries.GetTeam;
using WorkPulse.Application.Organizations.Queries.ListTeamMembers;
using WorkPulse.Application.Organizations.Queries.ListTeams;
using WorkPulse.API.Contracts.Organizations;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/teams")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class TeamsController(ISender sender) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<PagedList<TeamDto>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? sortBy = null, [FromQuery] string sortDirection = "asc", [FromQuery] bool includeArchived = false, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<TeamDto>>>)new ListTeamsQuery(new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}, new SortParams
		{
			SortBy = sortBy,
			SortDirection = sortDirection
		}, includeArchived), ct)).ToActionResult();
	}

	[HttpGet("{teamId:guid}")]
	public async Task<ActionResult<TeamDto>> Get(Guid teamId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<TeamDto>>)new GetTeamQuery(teamId), ct)).ToActionResult();
	}

	[HttpPost]
	public async Task<ActionResult<TeamDto>> Create([FromBody] CreateTeamRequest request, CancellationToken ct)
	{
		Result<TeamDto> result = await sender.Send((IRequest<Result<TeamDto>>)new CreateTeamCommand(request.Name, request.Key, request.Description, request.Icon, request.Color), ct);
		if (result.IsFailure)
		{
			return result.ToActionResult();
		}
		return CreatedAtAction("Get", new
		{
			teamId = result.Value.Id
		}, result.Value);
	}

	[HttpPut("{teamId:guid}")]
	public async Task<ActionResult<TeamDto>> Update(Guid teamId, [FromBody] UpdateTeamRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<TeamDto>>)new UpdateTeamCommand(teamId, request.Name, request.Description, request.Icon, request.Color), ct)).ToActionResult();
	}

	[HttpPost("{teamId:guid}/archive")]
	public async Task<IActionResult> Archive(Guid teamId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new ArchiveTeamCommand(teamId), ct)).ToActionResult();
	}

	[HttpGet("{teamId:guid}/members")]
	public async Task<ActionResult<PagedList<TeamMemberDto>>> ListMembers(Guid teamId, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<TeamMemberDto>>>)new ListTeamMembersQuery(teamId, new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}), ct)).ToActionResult();
	}

	[HttpPost("{teamId:guid}/members")]
	public async Task<ActionResult<TeamMemberDto>> AddMember(Guid teamId, [FromBody] AddTeamMemberRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<TeamMemberDto>>)new AddTeamMemberCommand(teamId, request.UserId, request.Role), ct)).ToActionResult();
	}

	[HttpPut("{teamId:guid}/members/{memberId:guid}")]
	public async Task<IActionResult> UpdateMember(Guid teamId, Guid memberId, [FromBody] UpdateTeamMemberRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new UpdateTeamMemberCommand(teamId, memberId, request.Role), ct)).ToActionResult();
	}

	[HttpDelete("{teamId:guid}/members/{memberId:guid}")]
	public async Task<IActionResult> RemoveMember(Guid teamId, Guid memberId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new RemoveTeamMemberCommand(teamId, memberId), ct)).ToActionResult();
	}
}
