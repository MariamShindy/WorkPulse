using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Search.Dtos;

namespace WorkPulse.Application.Search.Queries;

public sealed class SearchQueryHandler(ISearchReadService search, ITenantContext tenantContext) : IRequestHandler<SearchQuery, Result<IReadOnlyList<SearchResultDto>>>
{
	public async Task<Result<IReadOnlyList<SearchResultDto>>> Handle(SearchQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		return Result.Success((IReadOnlyList<SearchResultDto>)(await search.SearchAsync(tenantContext.TenantId, request.Query.Trim(), request.Scope, request.Limit, ct)).ToList());
	}
}
