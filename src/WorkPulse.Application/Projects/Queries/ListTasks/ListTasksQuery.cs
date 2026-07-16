using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Projects.Queries.ListTasks;

public sealed record ListTasksQuery(PaginationParams Pagination, Guid? TeamId = null, Guid? ProjectId = null, Guid? AssigneeId = null, Guid? WorkflowStateId = null, TaskPriority? Priority = null) : IRequest<Result<PagedList<TaskItemDto>>>, IBaseRequest, IQuery;
