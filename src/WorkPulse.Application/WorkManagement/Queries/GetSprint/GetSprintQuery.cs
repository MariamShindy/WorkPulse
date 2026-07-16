using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Queries.GetSprint;

public sealed record GetSprintQuery(Guid SprintId) : IRequest<Result<SprintDto>>, IBaseRequest, IQuery;
