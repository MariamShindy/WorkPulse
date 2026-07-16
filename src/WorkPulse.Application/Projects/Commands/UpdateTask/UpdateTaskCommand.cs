using System;
using System.Collections.Generic;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Projects.Commands.UpdateTask;

public sealed record UpdateTaskCommand(Guid TaskId, string Title, string? Description, TaskPriority Priority, Guid? ProjectId, Guid? AssigneeId, IReadOnlyList<Guid>? AssigneeIds, DateOnly? DueDate, int? StoryPoints, decimal? EstimatedHours, bool IsBlocked, string? BlockedReason, Guid? EpicId, Guid? SprintId, Guid? AssignedTeamId, byte[]? RowVersion) : IRequest<Result<TaskItemDto>>, IBaseRequest, ICommand;
