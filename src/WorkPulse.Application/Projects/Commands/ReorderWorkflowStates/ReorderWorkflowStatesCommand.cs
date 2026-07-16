using System;
using System.Collections.Generic;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Projects.Commands.ReorderWorkflowStates;

public sealed record ReorderWorkflowStatesCommand(Guid TeamId, IReadOnlyList<Guid> StateIdsInOrder) : IRequest<Result>, IBaseRequest, ICommand;
