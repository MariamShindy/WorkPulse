using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Organizations.Commands.UpdateTeamMember;

public sealed record UpdateTeamMemberCommand(Guid TeamId, Guid MemberId, TeamMemberRole Role) : IRequest<Result>, IBaseRequest, ICommand;
