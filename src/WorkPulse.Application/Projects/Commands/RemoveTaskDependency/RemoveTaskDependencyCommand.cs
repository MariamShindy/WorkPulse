using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Projects.Commands.RemoveTaskDependency;

public sealed record RemoveTaskDependencyCommand(Guid DependencyId) : IRequest<Result>, IBaseRequest, ICommand;
