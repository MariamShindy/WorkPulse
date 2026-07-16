using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Commands.CreateTeam;

public sealed record CreateTeamCommand(string Name, string? Key, string? Description, string? Icon, string? Color) : IRequest<Result<TeamDto>>, IBaseRequest, ICommand;
