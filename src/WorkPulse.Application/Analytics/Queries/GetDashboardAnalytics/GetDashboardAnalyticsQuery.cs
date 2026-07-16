using System;
using MediatR;
using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Analytics.Queries.GetDashboardAnalytics;

public sealed record GetDashboardAnalyticsQuery(Guid? TeamId = null, DateOnly? From = null, DateOnly? To = null) : IRequest<Result<DashboardAnalyticsDto>>, IBaseRequest, IQuery;
