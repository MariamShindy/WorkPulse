using System;
using System.Collections.Generic;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Queries.ListTaskLabels;

public sealed record ListTaskLabelsQuery(Guid TaskId) : IRequest<Result<IReadOnlyList<LabelDto>>>, IBaseRequest, IQuery;
