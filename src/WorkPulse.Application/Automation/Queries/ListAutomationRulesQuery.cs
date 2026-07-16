using MediatR;
using WorkPulse.Application.Automation.Dtos;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Automation.Queries;

public sealed record ListAutomationRulesQuery(PaginationParams Pagination, SortParams? Sort = null) : IRequest<Result<PagedList<AutomationRuleDto>>>, IBaseRequest, IQuery;
