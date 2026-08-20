using Microsoft.AspNetCore.Authorization;
using WorkPulse.API.Extensions;
using WorkPulse.Application.Auth.Commands.UpdateProfile;
using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Auth.Queries.GetProfile;
using WorkPulse.Application.Organizations.Commands.SetCurrentCompany;
using WorkPulse.Application.Organizations.Dtos;
using WorkPulse.Application.Organizations.Queries.ListMyCompanies;
using WorkPulse.API.Contracts.Auth;
using WorkPulse.API.Contracts.Organizations;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Policy = "RequireAuthenticated")]
public sealed class UsersController(ISender sender) : ControllerBase
{
	[HttpGet("profile")]
	[ProducesResponseType(typeof(UserProfileDto), 200)]
	public async Task<ActionResult<UserProfileDto>> GetProfile(CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<UserProfileDto>>)new GetProfileQuery(), ct)).ToActionResult();
	}

	[HttpPut("profile")]
	[ProducesResponseType(typeof(UserProfileDto), 200)]
	public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<UserProfileDto>>)new UpdateProfileCommand(request.FirstName, request.LastName, request.AvatarUrl), ct)).ToActionResult();
	}

	[HttpGet("me/companies")]
	[ProducesResponseType(typeof(IReadOnlyList<UserCompanyDto>), 200)]
	public async Task<ActionResult<IReadOnlyList<UserCompanyDto>>> ListMyCompanies(CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<IReadOnlyList<UserCompanyDto>>>)new ListMyCompaniesQuery(), ct)).ToActionResult();
	}

	[HttpPost("me/current-company")]
	[ProducesResponseType(typeof(CompanyDto), 200)]
	public async Task<ActionResult<CompanyDto>> SetCurrentCompany([FromBody] SetCurrentCompanyRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<CompanyDto>>)new SetCurrentCompanyCommand(request.CompanyId), ct)).ToActionResult();
	}
}
