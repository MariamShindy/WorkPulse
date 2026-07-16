using System;
using System.Collections.Generic;
using MediatR;
using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Analytics.Queries.GetAssigneeWorkload;

public sealed record GetAssigneeWorkloadQuery(Guid? TeamId = null) : IRequest<Result<IReadOnlyList<AssigneeWorkloadDto>>>, IBaseRequest, IQuery;
