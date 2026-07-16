using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.WorkManagement.Commands.UpdateEpic;

public sealed record UpdateEpicCommand(Guid EpicId, string Title, string? Description, EpicStatus Status) : IRequest<Result<EpicDto>>, IBaseRequest, ICommand;
