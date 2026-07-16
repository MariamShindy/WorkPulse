using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Organizations.Commands.ArchiveTeam;

public sealed record ArchiveTeamCommand(Guid TeamId) : IRequest<Result>, IBaseRequest, ICommand;
