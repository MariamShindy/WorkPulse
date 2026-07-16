using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Projects.Commands.DeleteWorkflowState;

public sealed record DeleteWorkflowStateCommand(Guid TeamId, Guid StateId) : IRequest<Result>, IBaseRequest, ICommand;
