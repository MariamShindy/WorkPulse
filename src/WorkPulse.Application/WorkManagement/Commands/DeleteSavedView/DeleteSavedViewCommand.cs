using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.WorkManagement.Commands.DeleteSavedView;

public sealed record DeleteSavedViewCommand(Guid SavedViewId) : IRequest<Result>, IBaseRequest, ICommand;
