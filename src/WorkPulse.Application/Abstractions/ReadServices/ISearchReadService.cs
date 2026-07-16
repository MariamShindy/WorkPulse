using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkPulse.Application.Search.Dtos;

namespace WorkPulse.Application.Abstractions.ReadServices;

public interface ISearchReadService
{
	Task<IReadOnlyList<SearchResultDto>> SearchAsync(Guid tenantId, string query, SearchScope scope, int limit, CancellationToken ct);
}
