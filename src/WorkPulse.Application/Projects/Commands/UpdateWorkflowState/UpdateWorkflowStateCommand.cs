using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Projects.Commands.UpdateWorkflowState;

public sealed record UpdateWorkflowStateCommand(Guid TeamId, Guid StateId, string Name, WorkflowStateType Type, string Color, int Position, bool IsDefault) : IRequest<Result<WorkflowStateDto>>, IBaseRequest, ICommand;
