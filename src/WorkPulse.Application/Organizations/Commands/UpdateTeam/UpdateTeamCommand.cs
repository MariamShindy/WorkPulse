using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Commands.UpdateTeam;

public sealed record UpdateTeamCommand(Guid TeamId, string Name, string? Description, string? Icon, string? Color) : IRequest<Result<TeamDto>>, IBaseRequest, ICommand;
