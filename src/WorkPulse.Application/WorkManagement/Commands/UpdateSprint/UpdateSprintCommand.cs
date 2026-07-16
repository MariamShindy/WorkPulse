using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.WorkManagement.Commands.UpdateSprint;

public sealed record UpdateSprintCommand(Guid SprintId, string Name, string? Goal, DateOnly StartDate, DateOnly EndDate, SprintStatus Status) : IRequest<Result<SprintDto>>, IBaseRequest, ICommand;
