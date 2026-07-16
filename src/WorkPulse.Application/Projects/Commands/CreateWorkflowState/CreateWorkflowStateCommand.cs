using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Projects.Commands.CreateWorkflowState;

public sealed record CreateWorkflowStateCommand(Guid TeamId, string Name, WorkflowStateType Type, string Color, int Position) : IRequest<Result<WorkflowStateDto>>, IBaseRequest, ICommand;
