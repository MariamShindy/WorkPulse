using System;
using MediatR;
using WorkPulse.Application.Automation.Dtos;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Automation.Commands;

public sealed record UpdateAutomationRuleCommand(Guid RuleId, string Name, AutomationTriggerType TriggerType, string TriggerConfigJson, AutomationActionType ActionType, string ActionConfigJson, bool IsEnabled) : IRequest<Result<AutomationRuleDto>>, IBaseRequest, ICommand;
