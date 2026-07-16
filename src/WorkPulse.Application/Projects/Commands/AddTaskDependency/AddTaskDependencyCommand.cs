using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Projects.Commands.AddTaskDependency;

public sealed record AddTaskDependencyCommand(Guid TaskId, Guid DependsOnTaskId, TaskDependencyType Type) : IRequest<Result<TaskDependencyDto>>, IBaseRequest, ICommand;
