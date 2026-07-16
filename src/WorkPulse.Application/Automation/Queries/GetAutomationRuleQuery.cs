using System;
using MediatR;
using WorkPulse.Application.Automation.Dtos;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Automation.Queries;

public sealed record GetAutomationRuleQuery(Guid RuleId) : IRequest<Result<AutomationRuleDto>>, IBaseRequest, IQuery;
