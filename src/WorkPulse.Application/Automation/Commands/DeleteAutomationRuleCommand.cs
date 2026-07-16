using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Automation.Commands;

public sealed record DeleteAutomationRuleCommand(Guid RuleId) : IRequest<Result>, IBaseRequest, ICommand;
