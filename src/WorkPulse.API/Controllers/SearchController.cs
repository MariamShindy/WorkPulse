using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Search.Dtos;
using WorkPulse.Application.Search.Queries;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/search")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class SearchController(ISender sender) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<IReadOnlyList<SearchResultDto>>> Search([FromQuery] string q, [FromQuery] SearchScope scope = SearchScope.All, [FromQuery] int limit = 25, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<IReadOnlyList<SearchResultDto>>>)new SearchQuery(q, scope, limit), ct)).ToActionResult();
	}
}
