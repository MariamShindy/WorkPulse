using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Search.Dtos;

namespace WorkPulse.Application.Search.Queries;

public sealed record SearchQuery(string Query, SearchScope Scope = SearchScope.All, int Limit = 25) : IRequest<Result<IReadOnlyList<SearchResultDto>>>, IBaseRequest, IQuery;
