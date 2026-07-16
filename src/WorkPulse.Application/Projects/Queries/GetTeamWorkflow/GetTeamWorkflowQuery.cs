using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Queries.GetTeamWorkflow;

public sealed record GetTeamWorkflowQuery(Guid TeamId) : IRequest<Result<WorkflowDto>>, IBaseRequest, IQuery;
