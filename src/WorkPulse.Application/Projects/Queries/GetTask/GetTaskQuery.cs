using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Queries.GetTask;

public sealed record GetTaskQuery(Guid TaskId) : IRequest<Result<TaskItemDto>>, IBaseRequest, IQuery;
