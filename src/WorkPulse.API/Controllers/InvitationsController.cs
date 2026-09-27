using Microsoft.AspNetCore.Authorization;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Auth.Commands.AcceptInvite;
using WorkPulse.Application.Auth.Commands.CancelInvitation;
using WorkPulse.Application.Auth.Commands.InviteUser;
using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Auth.Queries.ListInvitations;
using WorkPulse.API.Contracts.Auth;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/invitations")]
public sealed class InvitationsController(ISender sender) : ControllerBase
{
	[HttpPost]
	[Authorize(Policy = "RequireCompanyAdmin")]
	[TenantRequired]
	[ProducesResponseType(typeof(InvitationDto), 200)]
	public async Task<ActionResult<InvitationDto>> Invite([FromBody] InviteUserRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<InvitationDto>>)new InviteUserCommand(request.Email, request.Role), ct)).ToActionResult();
	}

	[HttpGet]
	[Authorize(Policy = "RequireCompanyAdmin")]
	[TenantRequired]
	[ProducesResponseType(typeof(IReadOnlyList<InvitationDto>), 200)]
	public async Task<ActionResult<IReadOnlyList<InvitationDto>>> List(CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<IReadOnlyList<InvitationDto>>>)new ListInvitationsQuery(), ct)).ToActionResult();
	}

	[HttpPost("accept")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(AuthResponseDto), 200)]
	public async Task<ActionResult<AuthResponseDto>> Accept([FromBody] AcceptInviteRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<AuthResponseDto>>)new AcceptInviteCommand(request.Token, request.Password, request.FirstName, request.LastName), ct)).ToActionResult();
	}

	[HttpDelete("{invitationId:guid}")]
	[Authorize(Policy = "RequireCompanyAdmin")]
	[TenantRequired]
	[ProducesResponseType(204)]
	public async Task<IActionResult> Cancel(Guid invitationId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new CancelInvitationCommand(invitationId), ct)).ToActionResult();
	}
}
