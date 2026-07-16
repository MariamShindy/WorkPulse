using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Organizations.Dtos;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Organizations.Commands.AddTeamMember;

public sealed record AddTeamMemberCommand(Guid TeamId, Guid UserId, TeamMemberRole Role) : IRequest<Result<TeamMemberDto>>, IBaseRequest, ICommand;
