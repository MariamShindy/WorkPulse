using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Projects.Commands.ArchiveProject;

public sealed record ArchiveProjectCommand(Guid ProjectId) : IRequest<Result>, IBaseRequest, ICommand;
