using System;
using System.Collections.Generic;
using MediatR;
using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Analytics.Queries.GetProjectProgress;

public sealed record GetProjectProgressQuery(Guid? TeamId = null) : IRequest<Result<IReadOnlyList<ProjectProgressDto>>>, IBaseRequest, IQuery;
