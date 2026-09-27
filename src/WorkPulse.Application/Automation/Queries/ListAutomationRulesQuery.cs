using WorkPulse.Application.Automation.Dtos;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;

namespace WorkPulse.Application.Automation.Queries;

public sealed record ListAutomationRulesQuery(PaginationParams Pagination, SortParams? Sort = null) : IRequest<Result<PagedList<AutomationRuleDto>>>, IBaseRequest, IQuery;
