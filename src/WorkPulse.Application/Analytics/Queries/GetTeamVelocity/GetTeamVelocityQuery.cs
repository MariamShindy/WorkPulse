using System;
using System.Collections.Generic;
using MediatR;
using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Analytics.Queries.GetTeamVelocity;

public sealed record GetTeamVelocityQuery(Guid? TeamId = null, int Weeks = 12) : IRequest<Result<IReadOnlyList<TeamVelocityPointDto>>>, IBaseRequest, IQuery;
