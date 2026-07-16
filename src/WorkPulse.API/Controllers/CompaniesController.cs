using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Organizations.Commands.AddCompanyMember;
using WorkPulse.Application.Organizations.Commands.CreateCompany;
using WorkPulse.Application.Organizations.Commands.UpdateCompany;
using WorkPulse.Application.Organizations.Commands.UpdateCompanyMember;
using WorkPulse.Application.Organizations.Dtos;
using WorkPulse.Application.Organizations.Queries.GetCompany;
using WorkPulse.Application.Organizations.Queries.ListCompanyMembers;

using WorkPulse.API.Contracts.Organizations;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/companies")]
[Authorize(Policy = "RequireAuthenticated")]
public sealed class CompaniesController(ISender sender) : ControllerBase
{
	[HttpPost]
	[AllowAnonymous]
	[ProducesResponseType(typeof(CompanyDto), 201)]
	public async Task<ActionResult<CompanyDto>> Create([FromBody] CreateCompanyRequest request, CancellationToken ct)
	{
		Result<CompanyDto> result = await sender.Send((IRequest<Result<CompanyDto>>)new CreateCompanyCommand(request.Name, request.Description, request.LogoUrl), ct);
		if (result.IsFailure)
		{
			return result.ToActionResult();
		}
		return Created($"/api/companies/{result.Value.Id}", result.Value);
	}

	[HttpGet("current")]
	[TenantRequired]
	[ProducesResponseType(typeof(CompanyDto), 200)]
	public async Task<ActionResult<CompanyDto>> GetCurrent(CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<CompanyDto>>)new GetCompanyQuery(), ct)).ToActionResult();
	}

	[HttpPut("current")]
	[TenantRequired]
	[ProducesResponseType(typeof(CompanyDto), 200)]
	public async Task<ActionResult<CompanyDto>> UpdateCurrent([FromBody] UpdateCompanyRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<CompanyDto>>)new UpdateCompanyCommand(request.Name, request.Description, request.LogoUrl), ct)).ToActionResult();
	}

	[HttpGet("current/members")]
	[TenantRequired]
	public async Task<ActionResult<PagedList<CompanyMemberDto>>> ListMembers([FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<CompanyMemberDto>>>)new ListCompanyMembersQuery(new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}), ct)).ToActionResult();
	}

	[HttpPost("current/members")]
	[TenantRequired]
	public async Task<ActionResult<CompanyMemberDto>> AddMember([FromBody] AddCompanyMemberRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<CompanyMemberDto>>)new AddCompanyMemberCommand(request.UserId, request.Role), ct)).ToActionResult();
	}

	[HttpPut("current/members/{memberId:guid}")]
	[TenantRequired]
	public async Task<IActionResult> UpdateMember(Guid memberId, [FromBody] UpdateCompanyMemberRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new UpdateCompanyMemberCommand(memberId, request.Role, request.IsActive), ct)).ToActionResult();
	}
}
